// Entities/Site.cs
namespace DocAnalytics.Domain.Entities;

public class Site
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
