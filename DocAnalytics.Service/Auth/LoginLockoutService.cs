using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Auth;

public sealed class LoginLockoutService : ILoginLockoutService
{
    private const int MaxFailures = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(15); // failures counted within this window
    private static readonly TimeSpan Lockout = TimeSpan.FromMinutes(15); // lock duration once tripped

    private readonly AppDbContext _db;
    public LoginLockoutService(AppDbContext db) => _db = db;

    private static string Norm(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public async Task<(bool Locked, int RetryAfterSeconds)> IsLockedAsync(string email, CancellationToken ct = default)
    {
        var e = Norm(email);
        var row = await _db.LoginAttempts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == e, ct);
        if (row?.LockedUntil is { } until && until > DateTime.UtcNow)
            return (true, (int)Math.Ceiling((until - DateTime.UtcNow).TotalSeconds));
        return (false, 0);
    }

    public async Task RegisterFailureAsync(string email, string? ip, CancellationToken ct = default)
    {
        var e = Norm(email);
        var now = DateTime.UtcNow;
        var row = await _db.LoginAttempts.FirstOrDefaultAsync(x => x.Email == e, ct);

        if (row is null)
        {
            row = new LoginAttempt { Id = Guid.NewGuid(), Email = e, FirstFailedAt = now };
            _db.LoginAttempts.Add(row);
        }

        // Start a fresh window if the last one expired and we're not currently locked.
        if (now - row.FirstFailedAt > Window && (row.LockedUntil is null || row.LockedUntil <= now))
        {
            row.FailedCount = 0;
            row.FirstFailedAt = now;
            row.LockedUntil = null;
        }

        row.FailedCount++;
        row.Ip = ip;
        row.LastFailedAt = now;

        if (row.FailedCount >= MaxFailures)
            row.LockedUntil = now.Add(Lockout);

        await _db.SaveChangesAsync(ct);
    }

    public async Task ResetAsync(string email, CancellationToken ct = default)
    {
        var e = Norm(email);
        var row = await _db.LoginAttempts.FirstOrDefaultAsync(x => x.Email == e, ct);
        if (row is not null)
        {
            _db.LoginAttempts.Remove(row);
            await _db.SaveChangesAsync(ct);
        }
    }
}
