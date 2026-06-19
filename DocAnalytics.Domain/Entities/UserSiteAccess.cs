// Entities/UserSiteAccess.cs
namespace DocAnalytics.Domain.Entities;

public class UserSiteAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SiteId { get; set; }
    public DateTime GrantedAt { get; set; }
    public User User { get; set; } = null!;
    public Site Site { get; set; } = null!;
}
