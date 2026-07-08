using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Tests.Support;

namespace DocAnalytics.Tests.Dashboard;

public class DashboardServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();

    private AppDbContext NewDb() =>
        TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    // ---- summary seed: a transaction carrying the four status counters ----
    private void SeedTx(AppDbContext db, int up, int proc, int comp, int fail,
        Guid? tenant = null, Guid? site = null)
    {
        db.Transactions.Add(new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenant ?? _tenant,
            SiteId = site ?? _site,
            State = "X",
            SourceSystem = "S3",
            UploadedCount = up,
            ProcessingCount = proc,
            CompletedCount = comp,
            FailedCount = fail,
            SubmittedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    // ---- recent-failures seed: Transaction + File + a Failed step ----
    private void SeedFailure(AppDbContext db, string fileName = "f.pdf", string step = "Validate",
        string errorCode = "E1", DateTime? at = null, Guid? tenant = null, Guid? site = null)
    {
        var when = at ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var tId = tenant ?? _tenant;
        var sId = site ?? _site;
        var txId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        db.Transactions.Add(new Transaction
        {
            Id = txId,
            TenantId = tId,
            SiteId = sId,
            State = "Failed",
            SourceSystem = "S3",
            SubmittedAt = when,
            LastUpdatedAt = when
        });
        db.Files.Add(new FileRecord
        {
            Id = fileId,
            TenantId = tId,
            SiteId = sId,
            TransactionId = txId,
            FileName = fileName,
            FileType = "pdf",
            Status = "Failed",
            CurrentStep = step,
            CreatedAt = when,
            LastUpdatedAt = when
        });
        db.FileStepHistory.Add(new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            StepName = step,
            Status = "Failed",
            StartedAt = when,
            CompletedAt = when,
            ErrorCode = errorCode,
            ErrorMessage = errorCode + " msg"
        });
    }

    private static RecentFailuresQuery RQ(int page = 1, int pageSize = 20,
        string? sortBy = null, string? sortDir = null)
        => new() { Page = page, PageSize = pageSize, SortBy = sortBy, SortDir = sortDir };

    // ================= Summary =================
    [Fact]
    public async Task GetSummary_sums_counters_across_transactions()
    {
        using var db = NewDb();
        SeedTx(db, up: 1, proc: 2, comp: 3, fail: 4);
        SeedTx(db, up: 1, proc: 1, comp: 1, fail: 1);
        await db.SaveChangesAsync();

        var s = await new DashboardService(db).GetSummaryAsync();

        Assert.Equal(2, s.Queued);
        Assert.Equal(3, s.InProgress);
        Assert.Equal(4, s.Completed);
        Assert.Equal(5, s.Failed);
        Assert.Equal(14, s.Total);
    }

    [Fact]
    public async Task GetSummary_returns_zeros_when_no_data()
    {
        using var db = NewDb();

        var s = await new DashboardService(db).GetSummaryAsync();

        Assert.Equal(0, s.Total);
    }

    [Fact]
    public async Task GetSummary_excludes_other_tenants()
    {
        using var db = NewDb();
        SeedTx(db, up: 1, proc: 0, comp: 0, fail: 0);                          // mine
        SeedTx(db, up: 9, proc: 9, comp: 9, fail: 9,
               tenant: Guid.NewGuid(), site: Guid.NewGuid());                  // theirs
        await db.SaveChangesAsync();

        var s = await new DashboardService(db).GetSummaryAsync();

        Assert.Equal(1, s.Queued);
        Assert.Equal(1, s.Total);
    }

    // ================= Recent Failures =================
    [Fact]
    public async Task GetRecentFailures_returns_failed_steps()
    {
        using var db = NewDb();
        SeedFailure(db, step: "Validate");
        await db.SaveChangesAsync();

        var r = await new DashboardService(db).GetRecentFailuresAsync(RQ());

        Assert.Equal(1, r.TotalCount);
        Assert.Equal("Validate", r.Items[0].FailedStep);
    }

    [Fact]
    public async Task GetRecentFailures_pages_results()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "a.pdf", errorCode: "1");
        SeedFailure(db, fileName: "b.pdf", errorCode: "2");
        SeedFailure(db, fileName: "c.pdf", errorCode: "3");
        await db.SaveChangesAsync();
        var svc = new DashboardService(db);

        var page1 = await svc.GetRecentFailuresAsync(RQ(page: 1, pageSize: 2));
        var page2 = await svc.GetRecentFailuresAsync(RQ(page: 2, pageSize: 2));

        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Single(page2.Items);
    }

    [Fact]
    public async Task GetRecentFailures_sorts_by_file_name_ascending()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "b.pdf", errorCode: "1");
        SeedFailure(db, fileName: "a.pdf", errorCode: "2");
        await db.SaveChangesAsync();

        var r = await new DashboardService(db)
            .GetRecentFailuresAsync(RQ(sortBy: "file_name", sortDir: "asc"));

        Assert.Equal("a.pdf", r.Items[0].FileName);
    }

    [Fact]
    public async Task GetRecentFailures_excludes_other_tenants()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "mine.pdf");
        SeedFailure(db, fileName: "theirs.pdf", tenant: Guid.NewGuid(), site: Guid.NewGuid());
        await db.SaveChangesAsync();

        var r = await new DashboardService(db).GetRecentFailuresAsync(RQ());

        Assert.Equal(1, r.TotalCount);
        Assert.Equal("mine.pdf", r.Items[0].FileName);
    }
}
