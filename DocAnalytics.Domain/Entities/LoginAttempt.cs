namespace DocAnalytics.Domain.Entities;

// Pre-auth brute-force tracker. NOT ITenantScoped — there is no tenant context before login.
public class LoginAttempt
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;   // stored lower-cased
    public string? Ip { get; set; }
    public int FailedCount { get; set; }
    public DateTime FirstFailedAt { get; set; }
    public DateTime LastFailedAt { get; set; }
    public DateTime? LockedUntil { get; set; }
}
