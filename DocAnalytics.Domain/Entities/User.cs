// Entities/User.cs
namespace DocAnalytics.Domain.Entities;

/// <summary>An application user. A platform super-admin (Developer) belongs to no tenant, so <see cref="TenantId"/> is nullable.</summary>
public class User
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning tenant; null for a Developer (platform super-admin).</summary>
    // NULLABLE now — a Developer (platform super-admin) belongs to no tenant.
    public Guid? TenantId { get; set; }

    /// <summary>Login email (unique).</summary>
    public string Email { get; set; } = null!;
    /// <summary>BCrypt password hash.</summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Role: Developer | Admin | Viewer.</summary>
    // Developer | Admin | Viewer
    public string Role { get; set; } = null!;

    // Provisioning fields (feature/roles-schema)
    /// <summary>Forces a password reset on first login.</summary>
    public bool MustChangePassword { get; set; }          // force reset on first login
    /// <summary>Whether the user is active (soft-delete instead of hard delete).</summary>
    public bool IsActive { get; set; } = true;            // soft-delete instead of hard delete
    /// <summary>The user who provisioned this account, if any.</summary>
    public Guid? CreatedBy { get; set; }                   // who provisioned this user
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>The owning tenant navigation property; null to match <see cref="TenantId"/>.</summary>
    public Tenant? Tenant { get; set; }                    // nullable to match TenantId
    /// <summary>The sites this user has been granted access to.</summary>
    public List<UserSiteAccess> SiteAccess { get; set; } = new();
}
