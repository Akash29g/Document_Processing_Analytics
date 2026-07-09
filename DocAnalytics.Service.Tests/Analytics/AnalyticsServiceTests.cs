using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Analytics;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Analytics;

public class AnalyticsServiceTests
{
    private static AnalyticsService Sut(FileRecord[] files)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.BuildMockDbSet().Object);
        return new AnalyticsService(ctx.Object);
    }

    private static FileRecord File(string status, DateTime lastUpdated, params FileStepHistory[] steps)
    {
        var f = new FileRecord { Id = Guid.NewGuid(), Status = status, LastUpdatedAt = lastUpdated, FileName = "f.pdf" };
        foreach (var s in steps) f.Steps.Add(s);
        return f;
    }

    [Fact]
    public async Task GetStatusDistributionAsync_counts_per_status_biggest_first()
    {
        var files = new[] { File("Completed", DateTime.UtcNow), File("Completed", DateTime.UtcNow), File("Failed", DateTime.UtcNow) };
        var result = await Sut(files).GetStatusDistributionAsync();
        Assert.Equal("Completed", result.Points[0].Label);
        Assert.Equal(2, result.Points[0].Value);
        Assert.Equal("Failed", result.Points[1].Label);
    }

    [Fact]
    public async Task GetThroughputAsync_counts_completed_per_day()
    {
        var d1 = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);
        var d2 = new DateTime(2026, 5, 2, 10, 0, 0, DateTimeKind.Utc);
        var files = new[] { File("Completed", d1), File("Completed", d1), File("Completed", d2), File("Failed", d1) };
        var result = await Sut(files).GetThroughputAsync(null, null);
        Assert.Equal(2, result.Points.Count);
        Assert.Equal("2026-05-01", result.Points[0].Label);
        Assert.Equal(2, result.Points[0].Value);
    }

    [Fact]
    public async Task GetTopErrorsAsync_ranks_error_codes()
    {
        var files = new[]
        {
            File("Failed", DateTime.UtcNow,
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Validate",  Status = "Failed", ErrorCode = "E1" },
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Transform", Status = "Failed", ErrorCode = "E1" },
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Load",      Status = "Failed", ErrorCode = "E2" },
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Upload",    Status = "Success", ErrorCode = null }),
        };
        var result = await Sut(files).GetTopErrorsAsync(5);
        Assert.Equal("E1", result.Points[0].Label);
        Assert.Equal(2, result.Points[0].Value);
        Assert.Equal("E2", result.Points[1].Label);
    }

    [Fact]
    public async Task GetErrorTrendAsync_counts_errors_per_day()
    {
        var d1 = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc);
        var files = new[]
        {
            File("Failed", DateTime.UtcNow,
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Validate", Status = "Failed", ErrorCode = "E1", StartedAt = d1 },
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Load",     Status = "Failed", ErrorCode = "E2", StartedAt = d1 }),
        };
        var result = await Sut(files).GetErrorTrendAsync(null, null);
        Assert.Single(result.Points);
        Assert.Equal("2026-05-01", result.Points[0].Label);
        Assert.Equal(2, result.Points[0].Value);
    }

    [Fact]
    public async Task GetStepPercentilesAsync_computes_durations()
    {
        var start = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc);
        var files = new[]
        {
            File("Completed", DateTime.UtcNow,
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Upload", Status = "Success", StartedAt = start, CompletedAt = start.AddSeconds(10) },
                new FileStepHistory { Id = Guid.NewGuid(), StepName = "Upload", Status = "Success", StartedAt = start, CompletedAt = start.AddSeconds(20) }),
        };
        var result = await Sut(files).GetStepPercentilesAsync();
        var upload = result.Single(r => r.Step == "Upload");
        Assert.Equal(2, upload.SampleCount);
        Assert.InRange(upload.P50Seconds, 10, 20);
    }
}
