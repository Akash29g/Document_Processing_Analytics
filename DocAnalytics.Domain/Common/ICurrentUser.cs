namespace DocAnalytics.Domain.Common;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid TenantId { get; }
    Guid SiteId { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}
