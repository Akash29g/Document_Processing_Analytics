using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Files;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Tests.Support;

namespace DocAnalytics.Tests.Files;

public class FileDetailsServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();
    private AppDbContext NewDb() => TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    private FileRecord SeedFile(AppDbContext db, string name = "invoice.pdf",
        string status = "Failed", string step = "Validate")
    {
        var when = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var txId = Guid.NewGuid();
        db.Transactions.Add(new Transaction { Id = txId, TenantId = _tenant, SiteId = _site, State = "Failed", SourceSystem = "S3", SubmittedAt = when, LastUpdatedAt = when });
        var file = new FileRecord { Id = Guid.NewGuid(), TenantId = _tenant, SiteId = _site, TransactionId = txId, FileName = name, FileType = "pdf", Status = status, CurrentStep = step, CreatedAt = when, LastUpdatedAt = when };
        db.Files.Add(file);
        return file;
    }

    // ---- FileDetailsService ----
    [Fact]
    public async Task GetFileDetails_returns_null_for_missing()
    {
        using var db = NewDb();
        var res = await new FileDetailsService(db).GetFileDetailsAsync(Guid.NewGuid());
        Assert.Null(res);   // 404 for not-found AND other-tenant (no existence leak)
    }

    [Fact]
    public async Task GetFileDetails_builds_timeline_with_error_and_fix()
    {
        using var db = NewDb();
        var file = SeedFile(db);
        db.FileStepHistory.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = file.Id, StepName = "Validate", Status = "Failed", StartedAt = file.CreatedAt, CompletedAt = file.CreatedAt, ErrorCode = "ERR_X", ErrorMessage = "bad" });
        db.ErrorCatalog.Add(new ErrorCatalog { Id = Guid.NewGuid(), ErrorCode = "ERR_X", Description = "d", RemediationMsg = "Fix it", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var res = await new FileDetailsService(db).GetFileDetailsAsync(file.Id);

        Assert.NotNull(res);
        Assert.Equal("invoice.pdf", res!.FileInfo.Name);
        Assert.Single(res.History);
        Assert.Equal("Fix it", res.History[0].Error!.SuggestedFix);
    }

    [Fact]
    public async Task GetFileLogs_returns_null_for_missing()
    {
        using var db = NewDb();
        Assert.Null(await new FileDetailsService(db).GetFileLogsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetFileLogs_contains_file_name_and_steps()
    {
        using var db = NewDb();
        var file = SeedFile(db);
        db.FileStepHistory.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = file.Id, StepName = "Upload", Status = "Success", StartedAt = file.CreatedAt, CompletedAt = file.CreatedAt });
        await db.SaveChangesAsync();

        var log = await new FileDetailsService(db).GetFileLogsAsync(file.Id);
        Assert.NotNull(log);
        Assert.Contains("invoice.pdf", log!.Content);
        Assert.Contains("Upload", log.Content);
    }

    // ---- InvoiceService ----
    [Fact]
    public async Task GetInvoice_returns_null_for_missing_file()
    {
        using var db = NewDb();
        Assert.Null(await new InvoiceService(db).GetInvoiceForFileAsync(Guid.NewGuid()));  // 404
    }

    [Fact]
    public async Task GetInvoice_computes_grand_total()
    {
        using var db = NewDb();
        var file = SeedFile(db, status: "Completed", step: "Load");
        db.InvoiceLineItems.AddRange(
            new InvoiceLineItem { Id = Guid.NewGuid(), TenantId = _tenant, SiteId = _site, FileId = file.Id, LineNumber = 1, Description = "A", LineTotal = 100.50m, IsValid = true, ExtractedAt = file.CreatedAt },
            new InvoiceLineItem { Id = Guid.NewGuid(), TenantId = _tenant, SiteId = _site, FileId = file.Id, LineNumber = 2, Description = "B", LineTotal = 49.50m, IsValid = true, ExtractedAt = file.CreatedAt });
        await db.SaveChangesAsync();

        var res = await new InvoiceService(db).GetInvoiceForFileAsync(file.Id);
        Assert.NotNull(res);
        Assert.Equal(2, res!.Items.Count);
        Assert.Equal(150.00m, res.GrandTotal);
    }

    [Fact]
    public async Task GetInvoice_empty_line_items_grand_total_zero()
    {
        using var db = NewDb();
        var file = SeedFile(db, status: "Completed", step: "Load");
        await db.SaveChangesAsync();

        var res = await new InvoiceService(db).GetInvoiceForFileAsync(file.Id);
        Assert.NotNull(res);              // file exists → not 404, just empty
        Assert.Empty(res!.Items);
        Assert.Equal(0m, res.GrandTotal);
    }
}
