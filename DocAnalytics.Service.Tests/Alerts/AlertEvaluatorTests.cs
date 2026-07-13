using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocAnalytics.Service.Tests.Alerts;

public class AlertEvaluatorTests
{
    private readonly Mock<IEmailSender> _email = new();

    private AlertEvaluator Sut(AppDbContext db) => new(db, _email.Object, Mock.Of<ILogger<AlertEvaluator>>());

    private static FileRecord File(Guid siteId, string status, DateTime updated) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        SiteId = siteId,
        TransactionId = Guid.NewGuid(),
        FileName = "f.pdf",
        FileType = "PDF",
        Status = status,
        CurrentStep = "Validate",
        LastUpdatedAt = updated,
        CreatedAt = updated
    };

    private static AlertRule Rule(Guid siteId, double threshold, bool enabled) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        SiteId = siteId,
        Name = "Rule",
        ThresholdPercent = threshold,
        WindowMinutes = 60,
        Email = "ops@acme.com",
        CooldownMinutes = 60,
        IsEnabled = enabled,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task EvaluateAllAsync_fires_when_failure_rate_exceeds_threshold()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var siteId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        db.AlertRules.Add(Rule(siteId, threshold: 10, enabled: true));
        for (var i = 0; i < 5; i++) db.Files.Add(File(siteId, "Failed", now.AddMinutes(-5)));
        for (var i = 0; i < 5; i++) db.Files.Add(File(siteId, "Completed", now.AddMinutes(-5)));
        db.SaveChanges();                                   // 5/10 = 50% > 10%

        await Sut(db).EvaluateAllAsync();

        _email.Verify(e => e.SendAsync("ops@acme.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(db.AlertRules.IgnoreQueryFilters().Single().LastTriggeredAt);
    }

    [Fact]
    public async Task EvaluateAllAsync_does_not_fire_under_threshold()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var siteId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        db.AlertRules.Add(Rule(siteId, threshold: 50, enabled: true));
        db.Files.Add(File(siteId, "Failed", now.AddMinutes(-5)));
        for (var i = 0; i < 9; i++) db.Files.Add(File(siteId, "Completed", now.AddMinutes(-5)));
        db.SaveChanges();                                   // 1/10 = 10% < 50%

        await Sut(db).EvaluateAllAsync();

        _email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Null(db.AlertRules.IgnoreQueryFilters().Single().LastTriggeredAt);
    }

    [Fact]
    public async Task EvaluateAllAsync_skips_disabled_rules()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var siteId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        db.AlertRules.Add(Rule(siteId, threshold: 10, enabled: false));
        for (var i = 0; i < 5; i++) db.Files.Add(File(siteId, "Failed", now.AddMinutes(-5)));
        db.SaveChanges();

        await Sut(db).EvaluateAllAsync();
        _email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
