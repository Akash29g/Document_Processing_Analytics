using System.Security.Cryptography;
using System.Text;
using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;          // IEmailSender
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DocAnalytics.Service.Auth;

/// <summary>
/// Default <see cref="IPasswordResetService"/>: generates a random one-time token, stores only its
/// SHA-256 hash, emails the raw token as a link, and consumes it to set a new BCrypt password hash.
/// </summary>
public class PasswordResetService : IPasswordResetService
{
    private readonly AppDbContext _db;
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly IEmailSender _email;
    private readonly string _frontendBaseUrl;

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(30);

    public PasswordResetService(
        AppDbContext db,
        IPasswordPolicy passwordPolicy,
        IEmailSender email,
        IConfiguration config)
    {
        _db = db;
        _passwordPolicy = passwordPolicy;
        _email = email;
        // e.g. "App:FrontendBaseUrl" in appsettings; falls back to local dev.
        _frontendBaseUrl = config["App:FrontendBaseUrl"] ?? "http://localhost:4200";
    }

    /// <inheritdoc />
    public async Task RequestResetAsync(ForgotPasswordRequest req, string? ip, CancellationToken ct)
    {
        var email = (req.Email ?? string.Empty).Trim();

        // Look up an active account. If none, we STILL return normally (no account enumeration).
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive, ct);
        if (user is null) return;

        // Invalidate any earlier unused tokens for this user (keep a single live token).
        var now = DateTime.UtcNow;
        var existing = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null && t.ExpiresAt > now)
            .ToListAsync(ct);
        foreach (var t in existing) t.UsedAt = now;

        // Generate raw token (goes in the email) + store only its hash.
        var rawToken = GenerateRawToken();
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash(rawToken),
            ExpiresAt = now.Add(TokenLifetime),
            CreatedAt = now,
            CreatedByIp = ip
        });
        await _db.SaveChangesAsync(ct);

        // Email the link.
        var link = $"{_frontendBaseUrl}/reset-password?token={rawToken}";
        var subject = "Reset your DocAnalytics password";
        var body =
            $"We received a request to reset your password.\n\n" +
            $"Click the link below to set a new password (valid for {TokenLifetime.TotalMinutes:N0} minutes):\n\n" +
            $"{link}\n\n" +
            $"If you didn't request this, you can safely ignore this email.";
        await _email.SendAsync(user.Email, subject, body, ct);
    }

    /// <inheritdoc />
    public async Task<string?> ResetPasswordAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Token))
            return "Invalid or expired reset link.";

        var now = DateTime.UtcNow;
        var hash = Hash(req.Token);

        var row = await _db.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash && t.UsedAt == null && t.ExpiresAt > now, ct);
        if (row is null)
            return "Invalid or expired reset link.";

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == row.UserId && u.IsActive, ct);
        if (user is null)
            return "Invalid or expired reset link.";

        // Enforce the same password rules as ChangePassword.
        var reason = await _passwordPolicy.ValidateAsync(req.NewPassword, ct);
        if (reason is not null) return reason;

        // Set new password, consume token (one-time), clear any forced-reset flag.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.MustChangePassword = false;
        row.UsedAt = now;

        // Security: revoke this user's active refresh tokens so old sessions can't continue.
        var activeRefresh = await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null && rt.ExpiresAt > now)
            .ToListAsync(ct);
        foreach (var rt in activeRefresh) rt.RevokedAt = now;

        await _db.SaveChangesAsync(ct);
        return null;   // success
    }

    // --- helpers ---

    private static string GenerateRawToken()
    {
        // 32 random bytes -> URL-safe base64 (no padding), safe to put in a link.
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static string Hash(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);   // fits the 88-char column, same idea as RefreshToken
    }
}
