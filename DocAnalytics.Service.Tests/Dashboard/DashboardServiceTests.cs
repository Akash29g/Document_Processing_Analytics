using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Dashboard;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_sums_counters_across_transactions()
    {
        var txns = new[]
        {
            new Transaction { Id = Guid.NewGuid(), UploadedCount = 2, ProcessingCount = 1, CompletedCount = 5, FailedCount = 1 },
            new Transaction { Id = Guid.NewGuid(), UploadedCount = 3, ProcessingCount = 0, CompletedCount = 4, FailedCount = 2 },
        };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Transactions).Returns(txns.BuildMockDbSet().Object);

        var summary = await new DashboardService(ctx.Object).GetSummaryAsync();

        Assert.Equal(5, summary.Queued);      // 2+3
        Assert.Equal(1, summary.InProgress);  // 1+0
        Assert.Equal(9, summary.Completed);   // 5+4
        Assert.Equal(3, summary.Failed);      // 1+2
        Assert.Equal(18, summary.Total);
    }

    [Fact]
    public async Task GetSummaryAsync_returns_zeros_when_no_transactions()
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Transactions).Returns(Array.Empty<Transaction>().BuildMockDbSet().Object);

        var summary = await new DashboardService(ctx.Object).GetSummaryAsync();

        Assert.Equal(0, summary.Total);
        Assert.Equal(0, summary.Queued);
    }

    [Fact]
    public async Task GetRecentFailuresAsync_returns_only_failed_steps()
    {
        var fileId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf" } };
        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1", CompletedAt = DateTime.UtcNow },
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Upload",   Status = "Success" },
        };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.BuildMockDbSet().Object);
        ctx.Setup(c => c.FileStepHistory).Returns(steps.BuildMockDbSet().Object);

        var result = await new DashboardService(ctx.Object).GetRecentFailuresAsync(new RecentFailuresQuery());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Validate", result.Items[0].FailedStep);
        Assert.Equal("a.pdf", result.Items[0].FileName);
    }

    [Fact]
    public async Task GetRecentFailuresAsync_pages_results()
    {
        var fileId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf" } };
        var steps = Enumerable.Range(0, 5).Select(i => new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            StepName = "Validate",
            Status = "Failed",
            ErrorCode = $"E{i}",
            CompletedAt = DateTime.UtcNow.AddMinutes(-i)
        }).ToArray();

        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.BuildMockDbSet().Object);
        ctx.Setup(c => c.FileStepHistory).Returns(steps.BuildMockDbSet().Object);

        var result = await new DashboardService(ctx.Object)
            .GetRecentFailuresAsync(new RecentFailuresQuery { Page = 1, PageSize = 2 });

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}
