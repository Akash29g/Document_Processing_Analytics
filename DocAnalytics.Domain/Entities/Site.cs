// Entities/Site.cs
namespace DocAnalytics.Domain.Entities;

/// <summary>A physical/logical site belonging to a tenant (not itself tenant-scoped; managed via provisioning).</summary>
public class Site
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The owning tenant.</summary>
    public Guid TenantId { get; set; }
    /// <summary>The site name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>The site location, if provided.</summary>
    public string? Location { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Whether the site is active (soft-delete flag).</summary>
    public bool IsActive { get; set; }
    /// <summary>The owning tenant navigation property.</summary>
    public Tenant Tenant { get; set; } = null!;
}
