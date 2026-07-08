using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Errors;
using DocAnalytics.Tests.Support;

namespace DocAnalytics.Tests.Errors;

public class ErrorServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();

    private AppDbContext NewDb() =>
        TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    // Seeds one Transaction + File + Step. status="Failed" makes it show up in the error list.
    private void SeedFailure(AppDbContext db, string fileName = "f.pdf", string step = "Validate",
        string source = "S3", string errorCode = "ERR_X", DateTime? failedAt = null,
        string status = "Failed", Guid? tenant = null, Guid? site = null)
    {
        var when = failedAt ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
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
            SourceSystem = source,
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
            Status = status,
            CurrentStep = step,
            CreatedAt = when,
            LastUpdatedAt = when
        });
        db.FileStepHistory.Add(new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            StepName = step,
            Status = status,
            StartedAt = when,
            CompletedAt = when,
            ErrorCode = errorCode,
            ErrorMessage = errorCode + " happened"
        });
    }

    private static ErrorListQuery Q(int page = 1, int pageSize = 20, string? step = null,
        string? source = null, DateTime? from = null, DateTime? to = null,
        string? sortBy = null, string? sortDir = null) => new()
        {
            Page = page,
            PageSize = pageSize,
            Step = step,
            Source = source,
            From = from,
            To = to,
            SortBy = sortBy,
            SortDir = sortDir,
        };

    [Fact]
    public async Task GetErrors_returns_only_failed_steps()
    {
        using var db = NewDb();
        SeedFailure(db, status: "Failed");
        SeedFailure(db, fileName: "ok.pdf", status: "Success");   // ignored
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q());

        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetErrors_filters_by_step()
    {
        using var db = NewDb();
        SeedFailure(db, step: "Validate", errorCode: "E_VAL");
        SeedFailure(db, step: "Load", errorCode: "E_LOAD");
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q(step: "Load"));

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Load", result.Items[0].Step);
    }

    [Fact]
    public async Task GetErrors_filters_by_source()
    {
        using var db = NewDb();
        SeedFailure(db, source: "S3", errorCode: "A");
        SeedFailure(db, source: "SFTP", errorCode: "B");
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q(source: "SFTP"));

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("SFTP", result.Items[0].Source);
    }

    [Fact]
    public async Task GetErrors_filters_by_from_date()
    {
        using var db = NewDb();
        SeedFailure(db, errorCode: "OLD", failedAt: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        SeedFailure(db, errorCode: "NEW", failedAt: new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc));
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q(from: new DateTime(2026, 1, 15)));

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("NEW", result.Items[0].ErrorCode);
    }

    [Fact]
    public async Task GetErrors_pages_results()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "a.pdf", errorCode: "1");
        SeedFailure(db, fileName: "b.pdf", errorCode: "2");
        SeedFailure(db, fileName: "c.pdf", errorCode: "3");
        await db.SaveChangesAsync();
        var svc = new ErrorService(db);

        var page1 = await svc.GetErrorsAsync(Q(page: 1, pageSize: 2));
        var page2 = await svc.GetErrorsAsync(Q(page: 2, pageSize: 2));

        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Single(page2.Items);
    }

    [Fact]
    public async Task GetErrors_sorts_by_file_name_ascending()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "b.pdf", errorCode: "1");
        SeedFailure(db, fileName: "a.pdf", errorCode: "2");
        await db.SaveChangesAsync();

        var result = await new ErrorService(db)
            .GetErrorsAsync(Q(sortBy: "file_name", sortDir: "asc"));

        Assert.Equal("a.pdf", result.Items[0].FileName);
    }

    [Fact]
    public async Task GetErrors_populates_suggested_fix_from_catalog()
    {
        using var db = NewDb();
        SeedFailure(db, errorCode: "ERR_X");
        db.ErrorCatalog.Add(new ErrorCatalog
        {
            Id = Guid.NewGuid(),
            ErrorCode = "ERR_X",
            Description = "desc",
            RemediationMsg = "Fix it",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q());

        Assert.Equal("Fix it", result.Items[0].SuggestedFix);
    }

    [Fact]
    public async Task GetErrors_suggested_fix_is_null_when_no_catalog_row()
    {
        using var db = NewDb();
        SeedFailure(db, errorCode: "NO_CATALOG");
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q());

        Assert.Null(result.Items[0].SuggestedFix);
    }

    [Fact]
    public async Task GetErrors_excludes_other_tenants()
    {
        using var db = NewDb();
        SeedFailure(db, errorCode: "MINE");
        SeedFailure(db, errorCode: "THEIRS", tenant: Guid.NewGuid(), site: Guid.NewGuid());
        await db.SaveChangesAsync();

        var result = await new ErrorService(db).GetErrorsAsync(Q());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("MINE", result.Items[0].ErrorCode);
    }

    [Fact]
    public async Task GetErrorsForExport_returns_all_rows_ignoring_paging()
    {
        using var db = NewDb();
        SeedFailure(db, fileName: "a.pdf", errorCode: "1");
        SeedFailure(db, fileName: "b.pdf", errorCode: "2");
        SeedFailure(db, fileName: "c.pdf", errorCode: "3");
        await db.SaveChangesAsync();

        var rows = await new ErrorService(db).GetErrorsForExportAsync(Q(pageSize: 2));

        Assert.Equal(3, rows.Count);   // export ignores pageSize
    }
}
