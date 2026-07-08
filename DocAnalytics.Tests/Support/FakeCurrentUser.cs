using DocAnalytics.Domain.Common;

namespace DocAnalytics.Tests.Support;

// Stand-in for the real logged-in user — drives the tenant/site global query filter.
public sealed class FakeCurrentUser : ICurrentUser
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid SiteId { get; init; }
    public string Role { get; init; } = "Admin";
    public bool IsAuthenticated => true;
}
