using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Batches;
using DocAnalytics.Tests.Support;

namespace DocAnalytics.Tests.Batches;

public class BatchServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();
    private AppDbContext NewDb() => TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    private Transaction SeedTx(AppDbContext db, string state = "Completed", string source = "S3",
        int total = 3, int done = 3, int fail = 0, DateTime? submitted = null,
        Guid? tenant = null, Guid? site = null)
    {
        var when = submitted ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenant ?? _tenant,
            SiteId = site ?? _site,
            State = state,
            SourceSystem = source,
            TotalFiles = total,
            UploadedCount = 0,
            ProcessingCount = 0,
            CompletedCount = done,
            FailedCount = fail,
            SubmittedAt = when,
            LastUpdatedAt = when
        };
        db.Transactions.Add(tx);
        return tx;
    }

    private static BatchListQuery Q(int page = 1, int pageSize = 20, string? status = null,
        string? source = null, string? search = null, string? sortBy = null, string? sortDir = null) =>
        new() { Page = page, PageSize = pageSize, Status = status, Source = source, Search = search, SortBy = sortBy, SortDir = sortDir };

    [Fact]
    public async Task GetBatches_returns_only_current_tenant()
    {
        using var db = NewDb();
        SeedTx(db);
        SeedTx(db, tenant: Guid.NewGuid(), site: Guid.NewGuid());
        await db.SaveChangesAsync();

        var res = await new BatchService(db).GetBatchesAsync(Q());
        Assert.Equal(1, res.TotalCount);
    }

    [Fact]
    public async Task GetBatches_filters_by_status()
    {
        using var db = NewDb();
        SeedTx(db, state: "Completed");
        SeedTx(db, state: "Failed");
        await db.SaveChangesAsync();

        var res = await new BatchService(db).GetBatchesAsync(Q(status: "failed"));
        Assert.Equal(1, res.TotalCount);
    }

    [Fact]
    public async Task GetBatches_filters_by_source()
    {
        using var db = NewDb();
        SeedTx(db, source: "S3");
        SeedTx(db, source: "SFTP");
        await db.SaveChangesAsync();

        var res = await new BatchService(db).GetBatchesAsync(Q(source: "SFTP"));
        Assert.Equal(1, res.TotalCount);
    }

    [Fact]
    public async Task GetBatches_pages_results()
    {
        using var db = NewDb();
        SeedTx(db); SeedTx(db); SeedTx(db);
        await db.SaveChangesAsync();

        var svc = new BatchService(db);
        var p1 = await svc.GetBatchesAsync(Q(page: 1, pageSize: 2));
        var p2 = await svc.GetBatchesAsync(Q(page: 2, pageSize: 2));

        Assert.Equal(3, p1.TotalCount);
        Assert.Equal(2, p1.Items.Count);
        Assert.Single(p2.Items);
    }

    [Fact]
    public async Task GetBatchById_returns_null_for_missing()
    {
        using var db = NewDb();
        var res = await new BatchService(db).GetBatchByIdAsync(Guid.NewGuid());
        Assert.Null(res);   // controller → 404
    }

    [Fact]
    public async Task GetBatchById_returns_detail()
    {
        using var db = NewDb();
        var tx = SeedTx(db, source: "S3");
        await db.SaveChangesAsync();

        var res = await new BatchService(db).GetBatchByIdAsync(tx.Id);
        Assert.NotNull(res);
        Assert.Equal("S3", res!.Source);
    }

    [Fact]
    public async Task GetSources_returns_distinct_sources()
    {
        using var db = NewDb();
        SeedTx(db, source: "S3");
        SeedTx(db, source: "S3");
        SeedTx(db, source: "SFTP");
        await db.SaveChangesAsync();

        var sources = await new BatchService(db).GetSourcesAsync();
        Assert.Equal(2, sources.Count);
        Assert.Contains("S3", sources);
        Assert.Contains("SFTP", sources);
    }
}
