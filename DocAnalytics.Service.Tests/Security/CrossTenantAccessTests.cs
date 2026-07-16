using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Files;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Service.Tests.Support;

namespace DocAnalytics.Service.Tests.Security;

public sealed class CrossTenantAccessTests
{
    [Fact]
    public async Task GetFileDetailsAsync_returns_null_for_other_tenant_file_id()
    {
        // ── Arrange ──
        var tenantA = Guid.NewGuid();
        var siteA = Guid.NewGuid();

        var tenantB = Guid.NewGuid();
        var siteB = Guid.NewGuid();

        using var db = InMemoryDb.Create(new TestCurrentUser
        {
            TenantId = tenantA,
            SiteId = siteA,
            Role = "Viewer",
        });

        var txnB = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            SiteId = siteB,
            State = "Processing",
            SourceSystem = "SAP",
            TotalFiles = 1,
            UploadedCount = 0,
            ProcessingCount = 1,
            FailedCount = 0,
            CompletedCount = 0,
            SubmittedAt = DateTime.UtcNow.AddMinutes(-10),
            LastUpdatedAt = DateTime.UtcNow,
            CompletedAt = null,
        };

        var fileB = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            SiteId = siteB,
            TransactionId = txnB.Id,
            FileName = "tenant-b.pdf",
            FileType = "pdf",
            Status = "Failed",
            CurrentStep = "Validate",
            FileSizeBytes = 1234,
            ExtractionStatus = null,
            ExtractionConfidence = null,
            StorageKey = "s3/key",
            CreatedAt = DateTime.UtcNow.AddMinutes(-9),
            LastUpdatedAt = DateTime.UtcNow.AddMinutes(-1),
        };

        db.Transactions.Add(txnB);
        db.Files.Add(fileB);

        // Risky table: NOT tenant-scoped (no tenant_id/site_id columns)
        db.FileStepHistory.Add(new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = fileB.Id,
            DocumentTypeId = null,
            StepName = "Validate",
            Status = "Failed",
            StartedAt = DateTime.UtcNow.AddMinutes(-5),
            CompletedAt = DateTime.UtcNow.AddMinutes(-4),
            ErrorCode = "ERR_X",
            ErrorMessage = "should not leak cross-tenant",
        });

        await db.SaveChangesAsync();

        var svc = new FileDetailsService(db);

        // ── Act ──
        var result = await svc.GetFileDetailsAsync(fileB.Id);

        // ── Assert ──
        // Service returns null => controller returns 404. This proves "no existence leak".
        Assert.Null(result);
    }

    [Fact]
    public async Task GetInvoiceForFileAsync_returns_null_for_other_tenant_file_id()
    {
        // ── Arrange ──
        var tenantA = Guid.NewGuid();
        var siteA = Guid.NewGuid();

        var tenantB = Guid.NewGuid();
        var siteB = Guid.NewGuid();

        using var db = InMemoryDb.Create(new TestCurrentUser
        {
            TenantId = tenantA,
            SiteId = siteA,
            Role = "Viewer",
        });

        var txnB = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            SiteId = siteB,
            State = "Completed",
            SourceSystem = "SAP",
            TotalFiles = 1,
            UploadedCount = 0,
            ProcessingCount = 0,
            FailedCount = 0,
            CompletedCount = 1,
            SubmittedAt = DateTime.UtcNow.AddMinutes(-10),
            LastUpdatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow.AddMinutes(-1),
        };

        var fileB = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            SiteId = siteB,
            TransactionId = txnB.Id,
            FileName = "invoice-tenant-b.pdf",
            FileType = "pdf",
            Status = "Completed",
            CurrentStep = "Load",
            FileSizeBytes = 5678,
            ExtractionStatus = "Success",
            ExtractionConfidence = 0.95m,
            StorageKey = "s3/key2",
            CreatedAt = DateTime.UtcNow.AddMinutes(-9),
            LastUpdatedAt = DateTime.UtcNow.AddMinutes(-1),
        };

        db.Transactions.Add(txnB);
        db.Files.Add(fileB);

        // Risky table: InvoiceHeader (in your repo it DOES implement ITenantScoped per repomix,
        // but we still prove "can't be reached cross-tenant").
        db.InvoiceHeaders.Add(new InvoiceHeader
        {
            Id = Guid.NewGuid(),
            FileId = fileB.Id,
            InvoiceNumber = "INV-B",
            InvoiceDate = "2026-01-01",
            Seller = "Seller B",
            Buyer = "Buyer B",
            Currency = "INR",
            Subtotal = 10m,
            Discount = 0m,
            Tax = 1m,
            Shipping = 0m,
            Total = 11m,
            ExtractedAt = DateTime.UtcNow,
            TenantId = tenantB,
            SiteId = siteB,
        });

        db.InvoiceLineItems.Add(new InvoiceLineItem
        {
            Id = Guid.NewGuid(),
            FileId = fileB.Id,
            TenantId = tenantB,
            SiteId = siteB,
            ItemCategoryId = null,
            LineNumber = 1,
            Description = "Should not leak",
            Quantity = 1,
            UnitPrice = 11m,
            LineTotal = 11m,
            Confidence = 0.9m,
            IsValid = true,
            ExtractedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();

        var svc = new InvoiceService(db);

        // ── Act ──
        var result = await svc.GetInvoiceForFileAsync(fileB.Id);

        // ── Assert ──
        Assert.Null(result);
    }
}
