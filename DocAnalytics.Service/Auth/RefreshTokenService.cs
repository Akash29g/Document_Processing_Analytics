using System.Security.Cryptography;
using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DocAnalytics.Service.Auth;

/// <summary>Default <see cref="IRefreshTokenService"/> implementation: issues/rotates/revokes opaque refresh tokens (hashes stored).</summary>
public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;
    private readonly int _lifetimeDays;

    public RefreshTokenService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _lifetimeDays = int.TryParse(config["Jwt:RefreshExpiryDays"], out var d) ? d : 7;
    }

    // 256 bits of entropy, URL-safe. This is the only time the raw token exists.
    private static string NewRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    // Store only the hash — a DB leak must not yield usable tokens.
    private static string Hash(string raw)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }

    /// <inheritdoc />
    public async Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(
        Guid userId, string? ip, string? userAgent = null, CancellationToken ct = default)
    {
        var raw = NewRawToken();
        var now = DateTime.UtcNow;
        var expiresAt = now.AddDays(_lifetimeDays);

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = Hash(raw),
            CreatedAt = now,
            ExpiresAt = expiresAt,
            CreatedByIp = ip,
            IpAddress = ip,
            UserAgent = userAgent,
            LastUsedAt = now,
        });
        await _db.SaveChangesAsync(ct);

        return (raw, expiresAt);
    }

    /// <inheritdoc />
    public async Task<(User User, string RawToken, DateTime ExpiresAt)?> ValidateAndRotateAsync(
        string rawToken, string? ip, string? userAgent = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken)) return null;

        var hash = Hash(rawToken);
        var existing = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (existing is null) return null;                       // unknown token

        // Reuse detection: a revoked token being presented → likely theft.
        // Nuke every active token for this user so both sides are forced to re-login.
        if (existing.RevokedAt is not null)
        {
            await RevokeAllForUserAsync(existing.UserId, ct);
            return null;
        }

        if (DateTime.UtcNow >= existing.ExpiresAt) return null;  // expired

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == existing.UserId && u.IsActive, ct);
        if (user is null) return null;                           // user gone/deactivated

        // Rotate: mint the replacement, then revoke the old one pointing at it.
        var raw = NewRawToken();
        var now = DateTime.UtcNow;
        var expiresAt = now.AddDays(_lifetimeDays);
        var newHash = Hash(raw);

        existing.LastUsedAt = now; // record use before it's superseded

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHash,
            CreatedAt = now,
            ExpiresAt = expiresAt,
            CreatedByIp = ip,
            IpAddress = ip,
            UserAgent = userAgent,
            LastUsedAt = now,
        });

        existing.RevokedAt = now;
        existing.ReplacedByTokenHash = newHash;

        await _db.SaveChangesAsync(ct);

        return (user, raw, expiresAt);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string rawToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken)) return;

        var hash = Hash(rawToken);
        var row = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (row is { RevokedAt: null })
        {
            row.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var active = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var t in active) t.RevokedAt = now;
        if (active.Count > 0) await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SessionDto>> ListActiveSessionsAsync(
        Guid userId, string? currentRawToken, CancellationToken ct = default)
    {
        var currentHash = string.IsNullOrEmpty(currentRawToken) ? null : Hash(currentRawToken);
        var now = DateTime.UtcNow;

        var sessions = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
            .OrderByDescending(t => t.LastUsedAt ?? t.CreatedAt)
            .ToListAsync(ct);

        return sessions.Select(t => new SessionDto(
            t.Id,
            DeviceLabelParser.Parse(t.UserAgent),
            t.IpAddress,
            t.CreatedAt,
            t.LastUsedAt,
            currentHash != null && t.TokenHash == currentHash
        )).ToList();
    }

    /// <inheritdoc />
    public async Task<bool> RevokeSessionAsync(Guid userId, Guid tokenId, CancellationToken ct = default)
    {
        // scoped by userId — this WHERE clause is the entire security guarantee here
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == tokenId && t.UserId == userId, ct);
        if (token is null || token.RevokedAt is not null) return false;

        token.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<int> RevokeAllOtherSessionsAsync(Guid userId, string currentRawToken, CancellationToken ct = default)
    {
        var currentHash = Hash(currentRawToken);
        var now = DateTime.UtcNow;

        var others = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.TokenHash != currentHash)
            .ToListAsync(ct);

        foreach (var t in others) t.RevokedAt = now;
        if (others.Count > 0) await _db.SaveChangesAsync(ct);
        return others.Count;
    }
}
