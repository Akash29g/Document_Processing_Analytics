using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;

namespace DocAnalytics.Service.Tests.Batches;

public class BatchServiceTests
{
    private static Transaction Txn(string state, string source, DateTime submitted)
        => new() { Id = Guid.NewGuid(), State = state, SourceSystem = source, SubmittedAt = submitted, LastUpdatedAt = submitted };

    private static BatchService Sut(Transaction[] txns, FileRecord[]? files = null)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Transactions).Returns(txns.BuildMockDbSet().Object);
        ctx.Setup(c => c.Files).Returns((files ?? Array.Empty<FileRecord>()).BuildMockDbSet().Object);
        return new BatchService(ctx.Object);
    }

    [Fact]
    public async Task GetBatchesAsync_returns_all_when_no_filters()
    {
        var result = await Sut(new[] { Txn("Completed", "SAP", DateTime.UtcNow), Txn("Failed", "CSV", DateTime.UtcNow) })
            .GetBatchesAsync(new BatchListQuery());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetBatchesAsync_filters_by_status()
    {
        var result = await Sut(new[] { Txn("Completed", "SAP", DateTime.UtcNow), Txn("Failed", "CSV", DateTime.UtcNow) })
            .GetBatchesAsync(new BatchListQuery { Status = "failed" });
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Failed", result.Items[0].State);
    }

    [Fact]
    public async Task GetBatchesAsync_filters_by_source()
    {
        var result = await Sut(new[] { Txn("Completed", "SAP", DateTime.UtcNow), Txn("Completed", "CSV", DateTime.UtcNow) })
            .GetBatchesAsync(new BatchListQuery { Source = "CSV" });
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("CSV", result.Items[0].SourceSystem);
    }

    [Fact]
    public async Task GetBatchesAsync_filters_by_date_range()
    {
        var txns = new[]
        {
            Txn("Completed", "SAP", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            Txn("Completed", "SAP", new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
        };
        var result = await Sut(txns).GetBatchesAsync(new BatchListQuery { From = new DateTime(2026, 5, 1), To = new DateTime(2026, 7, 1) });
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetBatchesAsync_pages_results()
    {
        var txns = Enumerable.Range(0, 5).Select(i => Txn("Completed", "SAP", DateTime.UtcNow.AddMinutes(-i))).ToArray();
        var result = await Sut(txns).GetBatchesAsync(new BatchListQuery { Page = 1, PageSize = 2 });
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetSourcesAsync_returns_distinct_sorted()
    {
        var txns = new[] { Txn("Completed", "SAP", DateTime.UtcNow), Txn("Failed", "CSV", DateTime.UtcNow), Txn("Completed", "SAP", DateTime.UtcNow) };
        Assert.Equal(new[] { "CSV", "SAP" }, await Sut(txns).GetSourcesAsync());
    }

    [Fact]
    public async Task GetBatchByIdAsync_returns_detail()
    {
        var txn = Txn("Failed", "SAP", DateTime.UtcNow);
        txn.TotalFiles = 3; txn.FailedCount = 1; txn.CompletedCount = 2;
        var result = await Sut(new[] { txn }).GetBatchByIdAsync(txn.Id);
        Assert.NotNull(result);
        Assert.Equal("Failed", result!.Status);
        Assert.Equal(1, result.FileStats.Failed);
        Assert.Equal(2, result.FileStats.Completed);
    }

    [Fact]
    public async Task GetBatchByIdAsync_returns_null_when_missing()
        => Assert.Null(await Sut(Array.Empty<Transaction>()).GetBatchByIdAsync(Guid.NewGuid()));

    [Fact]
    public async Task GetBatchFilesAsync_returns_null_when_batch_missing()
        => Assert.Null(await Sut(Array.Empty<Transaction>()).GetBatchFilesAsync(Guid.NewGuid(), new BatchFilesQuery()));

    [Fact]
    public async Task GetBatchFilesAsync_returns_paged_files()
    {
        var txn = Txn("Completed", "SAP", DateTime.UtcNow);
        var files = Enumerable.Range(0, 3).Select(i => new FileRecord
        {
            Id = Guid.NewGuid(),
            TransactionId = txn.Id,
            FileName = $"f{i}.pdf",
            FileType = "pdf",
            Status = "Completed",
            CurrentStep = "Load",
            CreatedAt = DateTime.UtcNow.AddMinutes(-i)
        }).ToArray();

        var result = await Sut(new[] { txn }, files).GetBatchFilesAsync(txn.Id, new BatchFilesQuery { Page = 1, PageSize = 2 });
        Assert.NotNull(result);
        Assert.Equal(3, result!.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetBatchesAsync_sorts_by_submitted_at_ascending()
    {
        var older = Txn("Completed", "SAP", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var newer = Txn("Completed", "SAP", new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        var result = await Sut(new[] { newer, older })
            .GetBatchesAsync(new BatchListQuery { SortBy = "submitted_at", SortDir = "asc" });
        Assert.Equal(older.Id, result.Items[0].TransactionId);
    }

    [Fact]
    public async Task GetBatchesAsync_sorts_by_total_files_descending()
    {
        var small = Txn("Completed", "SAP", DateTime.UtcNow); small.TotalFiles = 1;
        var big = Txn("Completed", "SAP", DateTime.UtcNow); big.TotalFiles = 9;
        var result = await Sut(new[] { small, big })
            .GetBatchesAsync(new BatchListQuery { SortBy = "total_files", SortDir = "desc" });
        Assert.Equal(big.Id, result.Items[0].TransactionId);
    }

    [Fact]
    public async Task GetBatchesAsync_sorts_by_state_ascending()
    {
        var completed = Txn("Completed", "SAP", DateTime.UtcNow);
        var failed = Txn("Failed", "SAP", DateTime.UtcNow);
        var result = await Sut(new[] { failed, completed })
            .GetBatchesAsync(new BatchListQuery { SortBy = "state", SortDir = "asc" });
        Assert.Equal("Completed", result.Items[0].State);
    }

}
