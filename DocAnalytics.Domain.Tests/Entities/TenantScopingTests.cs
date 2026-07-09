using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Domain.Tests.Entities;

public class TenantScopingTests
{
    // Regression guard: these MUST stay ITenantScoped, or the AppDbContext global
    // query filter silently stops isolating them per tenant/site (security risk).
    [Theory]
    [InlineData(typeof(Transaction))]
    [InlineData(typeof(ActivityLog))]
    [InlineData(typeof(InvoiceLineItem))]
    public void TenantScoped_entities_implement_marker(Type entityType)
        => Assert.True(typeof(ITenantScoped).IsAssignableFrom(entityType),
            $"{entityType.Name} must implement ITenantScoped for tenant isolation.");

    [Fact]
    public void TenantScoped_exposes_tenant_and_site_ids()
    {
        ITenantScoped e = new Transaction { TenantId = Guid.NewGuid(), SiteId = Guid.NewGuid() };
        Assert.NotEqual(Guid.Empty, e.TenantId);
        Assert.NotEqual(Guid.Empty, e.SiteId);
    }
}
