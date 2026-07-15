// Entities/User.cs
namespace DocAnalytics.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    // NULLABLE now — a Developer (platform super-admin) belongs to no tenant.
    public Guid? TenantId { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    // Developer | Admin | Viewer
    public string Role { get; set; } = null!;

    // Provisioning fields (feature/roles-schema)
    public bool MustChangePassword { get; set; }          // force reset on first login
    public bool IsActive { get; set; } = true;            // soft-delete instead of hard delete
    public Guid? CreatedBy { get; set; }                   // who provisioned this user
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant? Tenant { get; set; }                    // nullable to match TenantId
    public List<UserSiteAccess> SiteAccess { get; set; } = new();
}
