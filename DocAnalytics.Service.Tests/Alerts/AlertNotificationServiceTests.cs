using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Alerts;

public class AlertNotificationServiceTests
{
    private static AlertNotification Row(bool read, string sev = "warning") => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        SiteId = Guid.NewGuid(),
        AlertRuleId = Guid.NewGuid(),
        RuleName = "High failure rate",
        Message = "x",
        Severity = sev,
        IsRead = read,
        FiredAt = DateTime.UtcNow
    };

    [Fact]
    public async Task GetNotificationsAsync_unreadOnly_returns_only_unread()
    {
        var rows = new[] { Row(read: false), Row(read: true), Row(read: false) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.AlertNotifications).Returns(rows.ToList().BuildMockDbSet().Object);

        var unread = await new AlertNotificationService(ctx.Object)
            .GetNotificationsAsync(unreadOnly: true);

        Assert.Equal(2, unread.Count);
        Assert.All(unread, n => Assert.False(n.IsRead));
    }

    [Fact]
    public async Task GetNotificationsAsync_all_returns_everything()
    {
        var rows = new[] { Row(read: false), Row(read: true) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.AlertNotifications).Returns(rows.ToList().BuildMockDbSet().Object);

        var all = await new AlertNotificationService(ctx.Object)
            .GetNotificationsAsync(unreadOnly: false);

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task MarkReadAsync_sets_flag_and_timestamp()
    {
        var row = Row(read: false);
        var ctx = MockDb.Create();
        ctx.Setup(c => c.AlertNotifications).Returns(new[] { row }.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var ok = await new AlertNotificationService(ctx.Object).MarkReadAsync(row.Id);

        Assert.True(ok);
        Assert.True(row.IsRead);       // same object reference → mutated by the service
        Assert.NotNull(row.ReadAt);
    }

    [Fact]
    public async Task MarkReadAsync_returns_false_for_unknown_id()
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.AlertNotifications).Returns(new[] { Row(read: false) }.ToList().BuildMockDbSet().Object);

        var ok = await new AlertNotificationService(ctx.Object).MarkReadAsync(Guid.NewGuid());

        Assert.False(ok);
    }

    [Fact]
    public async Task MarkAllReadAsync_marks_every_unread_row()
    {
        var rows = new[] { Row(read: false), Row(read: false), Row(read: true) };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.AlertNotifications).Returns(rows.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var count = await new AlertNotificationService(ctx.Object).MarkAllReadAsync();

        Assert.Equal(2, count);
        Assert.All(rows, r => Assert.True(r.IsRead));
    }
}
