// Entities/Tenant.cs
namespace DocAnalytics.Domain.Entities;

/// <summary>A customer organization; the top of the tenancy hierarchy (owns sites and users).</summary>
public class Tenant
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The tenant display name.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Organization domain (e.g. "acme.com"); drives auto-generated user emails. Unique.</summary>
    // e.g. "acme.com" — drives auto-generated user emails (user@org.com). UNIQUE.
    public string OrgDomain { get; set; } = null!;

    /// <summary>Whether the tenant is active (soft-delete flag).</summary>
    public bool IsActive { get; set; } = true;
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Sites belonging to this tenant.</summary>
    public List<Site> Sites { get; set; } = new();
    /// <summary>Users belonging to this tenant.</summary>
    public List<User> Users { get; set; } = new();
}
