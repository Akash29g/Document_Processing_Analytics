// Entities/UserSiteAccess.cs
namespace DocAnalytics.Domain.Entities;

/// <summary>Join entity granting a user access to a specific site (many-to-many between users and sites).</summary>
public class UserSiteAccess
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The user being granted access.</summary>
    public Guid UserId { get; set; }
    /// <summary>The site the access applies to.</summary>
    public Guid SiteId { get; set; }
    /// <summary>When access was granted (UTC).</summary>
    public DateTime GrantedAt { get; set; }
    /// <summary>The user navigation property.</summary>
    public User User { get; set; } = null!;
    /// <summary>The site navigation property.</summary>
    public Site Site { get; set; } = null!;
}
