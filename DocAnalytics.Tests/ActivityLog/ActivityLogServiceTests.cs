using DocAnalytics.Data;
using DocAnalytics.Service.ActivityLog;
using DocAnalytics.Tests.Support;
using DomainActivityLog = DocAnalytics.Domain.Entities.ActivityLog;

namespace DocAnalytics.Tests.ActivityLog;

public class ActivityLogServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();
    private AppDbContext NewDb() => TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    private void Seed(AppDbContext db, string eventType = "FILE_STATE_CHANGED",
        string entityType = "File", string entityName = "invoice.pdf",
        DateTime? at = null, Guid? tenant = null, Guid? site = null)
    {
        var when = at ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        db.ActivityLog.Add(new DomainActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenant ?? _tenant,
            SiteId = site ?? _site,
            EventType = eventType,
            EntityType = entityType,
            EntityId = Guid.NewGuid(),
            EntityName = entityName,
            TriggeredBy = "system",
            CreatedAt = when
        });
    }

    private static ActivityLogQuery Q(int page = 1, int pageSize = 20, string? eventType = null,
        string? entityType = null, DateTime? from = null, DateTime? to = null,
        string? sortBy = null, string? sortDir = null) =>
        new() { Page = page, PageSize = pageSize, EventType = eventType, EntityType = entityType, From = from, To = to, SortBy = sortBy, SortDir = sortDir };

    [Fact]
    public async Task Filters_by_event_type_exact()
    {
        using var db = NewDb();
        Seed(db, eventType: "FILE_STATE_CHANGED");
        Seed(db, eventType: "BATCH_SUBMITTED");
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(eventType: "BATCH_SUBMITTED"), default);
        Assert.Equal(1, res.TotalCount);
    }

    [Fact]
    public async Task Filters_by_entity_type_exact()
    {
        using var db = NewDb();
        Seed(db, entityType: "File");
        Seed(db, entityType: "Batch");
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(entityType: "Batch"), default);
        Assert.Equal(1, res.TotalCount);
    }

    [Fact]
    public async Task Filters_by_date_range()
    {
        using var db = NewDb();
        Seed(db, entityName: "old.pdf", at: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        Seed(db, entityName: "new.pdf", at: new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc));
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(from: new DateTime(2026, 1, 15)), default);
        Assert.Equal(1, res.TotalCount);
        Assert.Equal("new.pdf", res.Items[0].Entity);
    }

    [Fact]
    public async Task Sorts_by_event_type_ascending()
    {
        using var db = NewDb();
        Seed(db, eventType: "B_EVENT");
        Seed(db, eventType: "A_EVENT");
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(sortBy: "event_type", sortDir: "asc"), default);
        Assert.Equal("A_EVENT", res.Items[0].EventType);
    }

    [Fact]
    public async Task Defaults_sort_newest_first()
    {
        using var db = NewDb();
        Seed(db, entityName: "old.pdf", at: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        Seed(db, entityName: "new.pdf", at: new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc));
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(), default);
        Assert.Equal("new.pdf", res.Items[0].Entity);   // desc default
    }

    [Fact]
    public async Task Pages_results()
    {
        using var db = NewDb();
        Seed(db); Seed(db); Seed(db);
        await db.SaveChangesAsync();

        var svc = new ActivityLogService(db);
        var p1 = await svc.GetActivityLogAsync(Q(page: 1, pageSize: 2), default);
        var p2 = await svc.GetActivityLogAsync(Q(page: 2, pageSize: 2), default);

        Assert.Equal(3, p1.TotalCount);
        Assert.Equal(2, p1.Items.Count);
        Assert.Single(p2.Items);
    }

    [Fact]
    public async Task Excludes_other_tenants()
    {
        using var db = NewDb();
        Seed(db, entityName: "mine.pdf");
        Seed(db, entityName: "theirs.pdf", tenant: Guid.NewGuid(), site: Guid.NewGuid());
        await db.SaveChangesAsync();

        var res = await new ActivityLogService(db).GetActivityLogAsync(Q(), default);
        Assert.Equal(1, res.TotalCount);
        Assert.Equal("mine.pdf", res.Items[0].Entity);
    }

    // NOTE: the `Entity` partial filter uses EF.Functions.ILike → NOT supported on
    // EFCore.InMemory. Cover it in a Postgres integration test, or skip here:
    // [Fact(Skip = "ILike unsupported on InMemory; needs real Postgres")]
    // public async Task Filters_by_entity_partial_match() { ... }
}
