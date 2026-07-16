namespace DocAnalytics.Domain.Entities;

// R4: DB-backed opaque refresh token. NOT ITenantScoped — this is auth-level,
// issued at login before any tenant/site context is meaningful for filtering.
// The raw token is NEVER stored; we persist only a SHA-256 hash of it.
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;   // SHA-256(base64url raw token)
    public DateTime ExpiresAt { get; set; }           // UTC
    public DateTime CreatedAt { get; set; }           // UTC
    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }          // set on logout / rotation / reuse-detected
    public string? ReplacedByTokenHash { get; set; }  // rotation chain (audit / reuse detection)

    // convenience (not mapped)
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
