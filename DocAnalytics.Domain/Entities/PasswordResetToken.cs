namespace DocAnalytics.Domain.Entities;

/// <summary>
/// DB-backed one-time password-reset token (Forgot Password flow).
/// NOT tenant-scoped — issued before any session/tenant context exists.
/// The raw token is never stored; only a SHA-256 hash is persisted.
/// </summary>
public class PasswordResetToken
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

    /// <summary>The IP that requested the reset, if captured.</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>When the token was consumed (UTC) — set once the password is reset. One-time use.</summary>
    public DateTime? UsedAt { get; set; }             // set on successful reset

    /// <summary>True when the token is neither used nor expired (not mapped to the database).</summary>
    // convenience (not mapped)
    public bool IsActive => UsedAt is null && DateTime.UtcNow < ExpiresAt;
}
