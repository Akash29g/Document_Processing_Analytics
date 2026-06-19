using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        if (await db.Tenants.AnyAsync()) return; // idempotent

        var now = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword("Password123!");

        var tenantA = new Tenant { Id = Guid.NewGuid(), Name = "Acme Corp", CreatedAt = now, IsActive = true };
        var tenantB = new Tenant { Id = Guid.NewGuid(), Name = "Globex Inc", CreatedAt = now, IsActive = true };

        var siteA = new Site { Id = Guid.NewGuid(), TenantId = tenantA.Id, Name = "Mumbai Plant", Location = "Mumbai, IN", CreatedAt = now, IsActive = true };
        var siteB = new Site { Id = Guid.NewGuid(), TenantId = tenantB.Id, Name = "Berlin DC", Location = "Berlin, DE", CreatedAt = now, IsActive = true };

        var userA = new User { Id = Guid.NewGuid(), TenantId = tenantA.Id, Email = "viewer@acme.com", PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true };
        var userB = new User { Id = Guid.NewGuid(), TenantId = tenantB.Id, Email = "viewer@globex.com", PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true };

        var accessA = new UserSiteAccess { Id = Guid.NewGuid(), UserId = userA.Id, SiteId = siteA.Id, GrantedAt = now };
        var accessB = new UserSiteAccess { Id = Guid.NewGuid(), UserId = userB.Id, SiteId = siteB.Id, GrantedAt = now };

        var docInvoice = new DocumentType { Id = Guid.NewGuid(), TypeName = "Invoice", Category = "PDF", IsActive = true, CreatedAt = now };
        var docManifest = new DocumentType { Id = Guid.NewGuid(), TypeName = "Manifest", Category = "CSV", IsActive = true, CreatedAt = now };

        var catGoods = new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "GOODS", CategoryName = "Goods", IsActive = true, CreatedAt = now };
        var catServices = new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SERVICES", CategoryName = "Services", IsActive = true, CreatedAt = now };

        var err = new ErrorCatalog
        {
            Id = Guid.NewGuid(),
            ErrorCode = "ERR_BAD_SCHEMA",
            Description = "CSV column headers do not match the expected schema.",
            RemediationMsg = "Check your column headers.",
            CreatedAt = now,
            UpdatedAt = now
        };

        var txnA = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA.Id,
            SiteId = siteA.Id,
            State = "Failed",
            SourceSystem = "S3_Bucket_Alpha",
            TotalFiles = 2,
            UploadedCount = 0,
            ProcessingCount = 0,
            FailedCount = 1,
            CompletedCount = 1,
            SubmittedAt = now.AddMinutes(-30),
            LastUpdatedAt = now
        };

        var fileOk = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA.Id,
            SiteId = siteA.Id,
            TransactionId = txnA.Id,
            DocumentTypeId = docInvoice.Id,
            FileName = "invoice_ok.pdf",
            FileType = "PDF",
            Status = "Completed",
            CurrentStep = "Load",
            FileSizeBytes = 12345,
            ExtractionStatus = "Done",
            ExtractionConfidence = 0.95m,
            LastUpdatedAt = now,
            CreatedAt = now.AddMinutes(-30)
        };
        var fileBad = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA.Id,
            SiteId = siteA.Id,
            TransactionId = txnA.Id,
            DocumentTypeId = docManifest.Id,
            FileName = "manifest_bad.csv",
            FileType = "CSV",
            Status = "Failed",
            CurrentStep = "Validate",
            FileSizeBytes = 678,
            ExtractionStatus = "Failed",
            ExtractionConfidence = 0.0m,
            LastUpdatedAt = now,
            CreatedAt = now.AddMinutes(-30)
        };

        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileBad.Id, DocumentTypeId = docManifest.Id, StepName = "Upload", Status = "Success", StartedAt = now.AddMinutes(-29), CompletedAt = now.AddMinutes(-29) },
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileBad.Id, DocumentTypeId = docManifest.Id, StepName = "Validate", Status = "Failed", StartedAt = now.AddMinutes(-28), CompletedAt = now.AddMinutes(-28), ErrorCode = "ERR_BAD_SCHEMA", ErrorMessage = "Unexpected column 'qty2'." }
        };

        var lineItems = new[]
        {
            new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileOk.Id, TenantId = tenantA.Id, SiteId = siteA.Id, ItemCategoryId = catGoods.Id,    LineNumber = 1, Description = "Steel bolts (box)", Quantity = 10m, UnitPrice = 5.50m, LineTotal = 55.00m, Confidence = 0.95m, IsValid = true, ExtractedAt = now },
            new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileOk.Id, TenantId = tenantA.Id, SiteId = siteA.Id, ItemCategoryId = catServices.Id, LineNumber = 2, Description = "Installation service", Quantity = 1m, UnitPrice = 120.00m, LineTotal = 120.00m, Confidence = 0.60m, IsValid = true, ExtractedAt = now }
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA.Id,
            SiteId = siteA.Id,
            EventType = "FILE_STATE_CHANGED",
            EntityType = "File",
            EntityId = fileBad.Id,
            EntityName = "manifest_bad.csv",
            OldState = "Processing",
            NewState = "Failed",
            TriggeredBy = "system",
            CreatedAt = now
        };

        // Tenant B (for isolation testing)
        var txnB = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB.Id,
            SiteId = siteB.Id,
            State = "Completed",
            SourceSystem = "SFTP_Beta",
            TotalFiles = 1,
            UploadedCount = 0,
            ProcessingCount = 0,
            FailedCount = 0,
            CompletedCount = 1,
            SubmittedAt = now.AddMinutes(-10),
            LastUpdatedAt = now,
            CompletedAt = now
        };
        var fileB = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB.Id,
            SiteId = siteB.Id,
            TransactionId = txnB.Id,
            DocumentTypeId = docInvoice.Id,
            FileName = "globex_invoice.pdf",
            FileType = "PDF",
            Status = "Completed",
            CurrentStep = "Load",
            LastUpdatedAt = now,
            CreatedAt = now.AddMinutes(-10)
        };

        db.AddRange(tenantA, tenantB, siteA, siteB, userA, userB, accessA, accessB,
                    docInvoice, docManifest, catGoods, catServices, err,
                    txnA, fileOk, fileBad, txnB, fileB, log);
        db.AddRange(steps);
        db.AddRange(lineItems);
        await db.SaveChangesAsync();
    }
}
