using DocAnalytics.Domain.Common;

namespace DocAnalytics.Performance.Tests.Support;

// Drives the tenant/site global query filter, same as Data.Tests' FakeCurrentUser.
public sealed class PerfCurrentUser : ICurrentUser
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid SiteId { get; init; }
    public string Role { get; init; } = "Viewer";
    public bool IsAuthenticated { get; init; } = true;
}
