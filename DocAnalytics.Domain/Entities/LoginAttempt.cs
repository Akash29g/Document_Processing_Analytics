namespace DocAnalytics.Domain.Entities;

/// <summary>Pre-auth brute-force tracker keyed by email. NOT tenant-scoped — there is no tenant context before login.</summary>
// Pre-auth brute-force tracker. NOT ITenantScoped — there is no tenant context before login.
public class LoginAttempt
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The email being tracked (stored lower-cased).</summary>
    public string Email { get; set; } = null!;   // stored lower-cased
    /// <summary>The originating IP, if captured.</summary>
    public string? Ip { get; set; }
    /// <summary>Number of consecutive failed attempts.</summary>
    public int FailedCount { get; set; }
    /// <summary>When the first failure in the current window occurred (UTC).</summary>
    public DateTime FirstFailedAt { get; set; }
    /// <summary>When the most recent failure occurred (UTC).</summary>
    public DateTime LastFailedAt { get; set; }
    /// <summary>Lockout expiry (UTC); null when not locked.</summary>
    public DateTime? LockedUntil { get; set; }
}
