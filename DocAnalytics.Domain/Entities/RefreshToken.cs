namespace DocAnalytics.Domain.Entities;

/// <summary>
/// DB-backed opaque refresh token (R4). NOT tenant-scoped — issued at login before tenant/site context applies.
/// The raw token is never stored; only a SHA-256 hash is persisted.
/// </summary>
// R4: DB-backed opaque refresh token. NOT ITenantScoped — this is auth-level,
// issued at login before any tenant/site context is meaningful for filtering.
// The raw token is NEVER stored; we persist only a SHA-256 hash of it.
public class RefreshToken
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>SHA-256 hash of the base64url raw token.</summary>
    public string TokenHash { get; set; } = null!;   // SHA-256(base64url raw token)
    /// <summary>Expiry timestamp (UTC).</summary>
    public DateTime ExpiresAt { get; set; }           // UTC
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }           // UTC
    /// <summary>The IP that created the token, if captured.</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>Raw User-Agent header captured at issuance/rotation time.</summary>
    public string? UserAgent { get; set; }

    /// <summary>Last-known client IP (updated on each rotation). CreatedByIp stays as the ORIGINAL issuance IP.</summary>
    public string? IpAddress { get; set; }

    /// <summary>When this token was last used (issued or rotated). Drives the "last active" column in the sessions UI.</summary>
    public DateTime? LastUsedAt { get; set; }


    /// <summary>When the token was revoked (UTC) — set on logout, rotation, or reuse detection.</summary>
    public DateTime? RevokedAt { get; set; }          // set on logout / rotation / reuse-detected
    /// <summary>Hash of the token that replaced this one (rotation chain for audit / reuse detection).</summary>
    public string? ReplacedByTokenHash { get; set; }  // rotation chain (audit / reuse detection)

    /// <summary>True when the token is neither revoked nor expired (not mapped to the database).</summary>
    // convenience (not mapped)
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
