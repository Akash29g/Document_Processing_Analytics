// Entities/Tenant.cs
namespace DocAnalytics.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    // e.g. "acme.com" — drives auto-generated user emails (user@org.com). UNIQUE.
    public string OrgDomain { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Site> Sites { get; set; } = new();
    public List<User> Users { get; set; } = new();
}
