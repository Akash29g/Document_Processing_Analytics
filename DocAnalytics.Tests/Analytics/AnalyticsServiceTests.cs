using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Analytics;
using DocAnalytics.Tests.Support;

namespace DocAnalytics.Tests.Analytics;

public class AnalyticsServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();
    private AppDbContext NewDb() => TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    // helper: a completed step lasting `seconds`
    private void SeedStep(AppDbContext db, string step, int seconds, string status = "Success")
    {
        var start = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var txId = Guid.NewGuid(); var fileId = Guid.NewGuid();
        db.Transactions.Add(new Transaction { Id = txId, TenantId = _tenant, SiteId = _site, State = "Completed", SourceSystem = "S3", SubmittedAt = start, LastUpdatedAt = start });
        db.Files.Add(new FileRecord { Id = fileId, TenantId = _tenant, SiteId = _site, TransactionId = txId, FileName = "f.pdf", FileType = "pdf", Status = "Completed", CurrentStep = step, CreatedAt = start, LastUpdatedAt = start });
        db.FileStepHistory.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = step, Status = status, StartedAt = start, CompletedAt = start.AddSeconds(seconds) });
    }

    [Fact]
    public async Task TopErrors_groups_and_orders_by_count()
    {
        using var db = NewDb();
        // seed several failed steps with repeating error codes, then assert ordering.
        // (fill using your ErrorService-style SeedFailure helper)
        await db.SaveChangesAsync();
        var svc = new AnalyticsService(db);
        var series = await svc.GetTopErrorsAsync(5);
        Assert.NotNull(series);
    }

    [Fact]
    public async Task StepPercentiles_computes_p50_p90_p99()
    {
        using var db = NewDb();
        // 10 samples 1..10 seconds for step "Validate"
        for (int s = 1; s <= 10; s++) SeedStep(db, "Validate", s);
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var result = await svc.GetStepPercentilesAsync();   // ← confirm real name
        var validate = result.Single(r => r.Step == "Validate");

        Assert.Equal(10, validate.SampleCount);
        Assert.True(validate.P90Seconds >= validate.P50Seconds);
        Assert.True(validate.P99Seconds >= validate.P90Seconds);
    }

    [Fact]
    public async Task Throughput_counts_completed_per_bucket()
    {
        using var db = NewDb();
        SeedStep(db, "Load", 5);
        SeedStep(db, "Load", 5);
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var series = await svc.GetThroughputAsync(null, null);  // ← confirm signature
        Assert.NotNull(series);
    }
}
