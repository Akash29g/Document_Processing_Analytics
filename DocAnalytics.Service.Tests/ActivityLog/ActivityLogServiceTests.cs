using DocAnalytics.Service.ActivityLog;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using DomainActivityLog = DocAnalytics.Domain.Entities.ActivityLog;

namespace DocAnalytics.Service.Tests.ActivityLog;

public class ActivityLogServiceTests
{
    private static DomainActivityLog Row(string eventType, string entityType, DateTime createdAt) =>
        new() { Id = Guid.NewGuid(), EventType = eventType, EntityType = entityType, TriggeredBy = "system", CreatedAt = createdAt };

    [Fact]
    public async Task GetActivityLogAsync_filters_by_event_type()
    {
        var rows = new[] { Row("BATCH_SUBMITTED", "Batch", DateTime.UtcNow), Row("FILE_STATE_CHANGED", "File", DateTime.UtcNow) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(rows.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(new ActivityLogQuery { EventType = "BATCH_SUBMITTED" });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("BATCH_SUBMITTED", result.Items[0].EventType);
    }

    [Fact]
    public async Task GetActivityLogAsync_orders_newest_first_by_default()
    {
        var older = Row("X", "File", DateTime.UtcNow.AddHours(-1));
        var newer = Row("Y", "File", DateTime.UtcNow);
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(new[] { older, newer }.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(new ActivityLogQuery());

        Assert.Equal("Y", result.Items[0].EventType);   // newest first
    }

    [Fact]
    public async Task GetActivityLogAsync_filters_by_date_range()
    {
        var rows = new[]
        {
            Row("A", "File", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            Row("B", "File", new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
        };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(rows.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(
            new ActivityLogQuery { From = new DateTime(2026, 5, 1), To = new DateTime(2026, 7, 1) });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("B", result.Items[0].EventType);
    }

    [Fact]
    public async Task GetActivityLogAsync_pages_results()
    {
        var rows = Enumerable.Range(0, 5).Select(i => Row($"E{i}", "File", DateTime.UtcNow.AddMinutes(-i))).ToArray();
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(rows.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(new ActivityLogQuery { Page = 1, PageSize = 2 });

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
    [Fact]
    public async Task GetActivityLogAsync_filters_by_entity_type()
    {
        var rows = new[] { Row("X", "Batch", DateTime.UtcNow), Row("Y", "File", DateTime.UtcNow) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(rows.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(new ActivityLogQuery { EntityType = "Batch" });
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Batch", result.Items[0].EntityType);
    }

    [Fact]
    public async Task GetActivityLogAsync_sorts_by_event_type_ascending()
    {
        var rows = new[] { Row("Zeta", "File", DateTime.UtcNow), Row("Alpha", "File", DateTime.UtcNow) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.ActivityLog).Returns(rows.BuildMockDbSet().Object);

        var result = await new ActivityLogService(ctx.Object).GetActivityLogAsync(new ActivityLogQuery { SortBy = "event_type", SortDir = "asc" });
        Assert.Equal("Alpha", result.Items[0].EventType);
    }

}
