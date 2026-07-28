namespace DocAnalytics.Domain.Entities;

/// <summary>A single-use TOTP backup code, BCrypt-hashed like passwords. 8-10 are generated at setup time.</summary>
public class TwoFactorRecoveryCode
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>The owning user.</summary>
    public Guid UserId { get; set; }

    /// <summary>BCrypt hash of the recovery code. Plaintext is shown once at generation time and never stored.</summary>
    public string CodeHash { get; set; } = null!;

    /// <summary>When this code was consumed; null while still usable.</summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
