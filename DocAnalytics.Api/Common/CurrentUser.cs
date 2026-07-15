using DocAnalytics.Domain.Common;

namespace DocAnalytics.Api.Common;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid SiteId { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public bool IsAuthenticated { get; private set; }

    public void Set(Guid userId, Guid tenantId, Guid siteId, string role)
    {
        UserId = userId; TenantId = tenantId; SiteId = siteId; Role = role; IsAuthenticated = true;
    }
}
