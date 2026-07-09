using DocAnalytics.Domain.Entities;
using DocAnalytics.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Tests.Persistence;

public class TenantIsolationTests
{
    private static Transaction Tx(Guid tenant, Guid site) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenant,
        SiteId = site,
        State = "Completed",
        SourceSystem = "S3",
        SubmittedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow,
    };

    [Fact]
    public async Task Query_returns_only_current_tenant_rows()
    {
        var tenantA = Guid.NewGuid();
        var siteX = Guid.NewGuid();
        using var db = TestDb.Create(new FakeCurrentUser { TenantId = tenantA, SiteId = siteX });

        db.Transactions.AddRange(
            Tx(tenantA, siteX),                       // mine
            Tx(Guid.NewGuid(), Guid.NewGuid()));      // another tenant
        await db.SaveChangesAsync();

        var rows = await db.Transactions.ToListAsync();

        Assert.Single(rows);
        Assert.Equal(tenantA, rows[0].TenantId);
    }

    [Fact]
    public async Task Query_excludes_same_tenant_different_site()
    {
        var tenant = Guid.NewGuid();
        var siteX = Guid.NewGuid();
        var siteY = Guid.NewGuid();
        using var db = TestDb.Create(new FakeCurrentUser { TenantId = tenant, SiteId = siteX });

        db.Transactions.AddRange(Tx(tenant, siteX), Tx(tenant, siteY));
        await db.SaveChangesAsync();

        var rows = await db.Transactions.ToListAsync();

        Assert.Single(rows);
        Assert.Equal(siteX, rows[0].SiteId);
    }
}
