// Entities/User.cs
namespace DocAnalytics.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;     // Admin | Viewer
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public ICollection<UserSiteAccess> SiteAccess { get; set; } = new List<UserSiteAccess>();
}
