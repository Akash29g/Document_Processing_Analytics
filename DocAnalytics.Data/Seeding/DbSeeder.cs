using System.Diagnostics.CodeAnalysis;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data.Seeding;

[ExcludeFromCodeCoverage]
public static class DbSeeder
{
    // ─────────────────────────────────────────────────────────────
    //  FIXED IDs — stable across resets so tokens & site_id never go stale.
    //  Login: Password123!  for every DEMO user (dev only).
    //  (see original comments for tenant/site/user layout)
    // ─────────────────────────────────────────────────────────────
    private static readonly Guid DeveloperId = new("6a1f2c3d-8e4b-4a9c-b2d1-5f7e9a0c1b34");
    private static readonly Guid AcmeId = new("2d9f83a1-7c4e-4b6a-9f21-3e5d8c0a1b62");
    private static readonly Guid GlobexId = new("8b3c1e7d-45a9-4f2b-8d63-1a2c4e6f9b05");
    private static readonly Guid AcmeMumbai = new("4e7a9c2f-1b3d-4a8e-9c5f-2d6b8f0a3c14");
    private static readonly Guid AcmeDelhi = new("5f8b0d3a-2c4e-4b9f-8d16-3e7c9a1b4d25");
    private static readonly Guid AcmeChennai = new("6a9c1e4b-3d5f-4c0a-9e27-4f8d0b2c5e36");
    private static readonly Guid AcmePune = new("7b0d2f5c-4e6a-4d1b-8f38-5a9e1c3d6f47");
    private static readonly Guid AcmeKolkata = new("8c1e3a6d-5f7b-4e2c-9a49-6b0f2d4e7a58");
    private static readonly Guid GlobexBerlin = new("9d2f4b7e-6a8c-4f3d-8b5a-7c1a3e5f8b69");
    private static readonly Guid GlobexMunich = new("0e3a5c8f-7b9d-4a4e-9c6b-8d2b4f6a9c70");
    private static readonly Guid AcmeUserA = new("1f4b6d9a-8c0e-4b5f-8d7c-9e3c5a7b0d81");
    private static readonly Guid AcmeUserB = new("2a5c7e0b-9d1f-4c6a-9e8d-0f4d6b8c1e92");
    private static readonly Guid AcmeUserD = new("3b6d8f1c-0e2a-4d7b-8f9e-1a5e7c9d2f03");
    private static readonly Guid AcmeUserE = new("4c7e9a2d-1f3b-4e8c-9a0f-2b6f8d0e3a14");
    private static readonly Guid AcmeUserF = new("5d8f0b3e-2a4c-4f9d-8b1a-3c7a9e1f4b25");
    private static readonly Guid AcmeUserG = new("6e9a1c4f-3b5d-4a0e-9c2b-4d8b0f2a5c36");
    private static readonly Guid AdminAcme = new("7f0b2d5a-4c6e-4b1f-8d3c-5e9c1a3b6d47");
    private static readonly Guid GlobexUserC = new("8a1c3e6b-5d7f-4c2a-9e4d-6f0d2b4c7e58");
    private static readonly Guid AdminGlobex = new("9b2d4f7c-6e8a-4d3b-8f5e-7a1e3c5d8f69");

    private static readonly (string Code, string Desc, string Remediation, string Msg)[] ErrorDefs =
    {
        ("ERR_BAD_SCHEMA",        "CSV column headers do not match the expected schema.", "Check your column headers against the template.", "Unexpected column 'qty2'."),
        ("ERR_TIMEOUT",           "Processing step exceeded the allotted time.",          "Retry; if it persists, reduce the file size.",    "Step timed out after 300s."),
        ("ERR_CORRUPT_FILE",      "The uploaded file is corrupt or unreadable.",          "Re-export the document and upload again.",        "Unable to open file: unexpected EOF."),
        ("ERR_OCR_LOW_CONFIDENCE","Extraction confidence below the accepted threshold.",  "Upload a higher-resolution scan.",                "OCR confidence 0.42 < 0.70 threshold."),
        ("ERR_MISSING_FIELD",     "A required field was missing from the document.",       "Ensure all mandatory fields are present.",        "Required field 'invoice_total' not found."),
        ("ERR_UNSUPPORTED_FORMAT","The file format is not supported.",                     "Convert the file to PDF or CSV.",                 "Format '.docx' is not supported."),
        ("ERR_DUPLICATE",         "A document with the same hash already exists.",         "Remove the duplicate before re-submitting.",      "Duplicate of an existing file."),
        ("ERR_AUTH_UPSTREAM",     "Authentication with an upstream service failed.",       "Renew upstream credentials and retry.",           "Upstream returned 401 Unauthorized."),
        ("ERR_BEDROCK_LOWCONF",   "Extraction confidence below the accepted threshold.",  "Re-upload a clearer PDF or higher-resolution JPEG; verify that totals match line items.",         "Confidence 0.60 < 0.70 threshold."),
        ("ERR_UNREADABLE",        "Nova could not read core fields from the document.",   "Ensure the PDF isn't a blank/garbled scan.",      "Seller/total missing."),
        ("ERR_EXTRACTION_FAILED", "The extraction step threw an unexpected error.",       "Retry; if it persists, check Bedrock access.",    "Bedrock call failed."),
        ("ERR_MALWARE_DETECTED",  "Malware detected by security scan.",                   "The file failed the malware scan and was removed. Verify the document source and upload a clean copy.", "GuardDuty scan verdict: THREATS_FOUND."),
        ("ERR_INVALID_FILETYPE",  "File content does not match its extension.",           "The uploaded file is not a genuine PDF. Re-export the document as PDF and upload again.",               "Magic-byte check failed: not a PDF."),
    };

    /// <summary>Orchestrator for dev/tests: migrate + catalogs + demo. Prod uses the granular methods.</summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        await SeedCatalogsAsync(db);
        await SeedDemoDataAsync(db);
    }   // 🔧 FIX 1: this closing brace was missing

    /// <summary>Reference data safe for ALL environments (no credentials). Idempotent per catalog.</summary>
    public static async Task SeedCatalogsAsync(AppDbContext db)
    {
        var now = DateTime.UtcNow;

        // ── DocumentType catalog ──
        if (!await db.Set<DocumentType>().AnyAsync())
        {
            db.AddRange(new[]
            {
                new DocumentType { Id = Guid.NewGuid(), TypeName = "Invoice",       Category = "PDF", IsActive = true, CreatedAt = now },
                new DocumentType { Id = Guid.NewGuid(), TypeName = "Manifest",      Category = "CSV", IsActive = true, CreatedAt = now },
                new DocumentType { Id = Guid.NewGuid(), TypeName = "PurchaseOrder", Category = "PDF", IsActive = true, CreatedAt = now },
                new DocumentType { Id = Guid.NewGuid(), TypeName = "Receipt",       Category = "PDF", IsActive = true, CreatedAt = now },
                new DocumentType { Id = Guid.NewGuid(), TypeName = "PackingSlip",   Category = "CSV", IsActive = true, CreatedAt = now },
                new DocumentType { Id = Guid.NewGuid(), TypeName = "BillOfLading",  Category = "PDF", IsActive = true, CreatedAt = now },
            });
        }

        // ── ItemCategory catalog ──
        if (!await db.Set<ItemCategory>().AnyAsync())
        {
            db.AddRange(new[]
            {
                new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "GOODS",    CategoryName = "Goods",    IsActive = true, CreatedAt = now },
                new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SERVICES", CategoryName = "Services", IsActive = true, CreatedAt = now },
                new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SHIPPING", CategoryName = "Shipping", IsActive = true, CreatedAt = now },
                new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "TAX",      CategoryName = "Tax",      IsActive = true, CreatedAt = now },
                new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "MISC",     CategoryName = "Misc",     IsActive = true, CreatedAt = now },
            });
        }

        // ── ErrorCatalog: insert any codes not already present (covers first-seed AND backfill) ──
        var existingCodes = await db.ErrorCatalog.Select(x => x.ErrorCode).ToListAsync();
        var toAdd = ErrorDefs
            .Where(e => !existingCodes.Contains(e.Code))
            .Select(e => new ErrorCatalog
            {
                Id = Guid.NewGuid(),
                ErrorCode = e.Code,
                Description = e.Desc,
                RemediationMsg = e.Remediation,
                CreatedAt = now,
                UpdatedAt = now
            })
            .ToArray();
        if (toAdd.Length > 0) db.AddRange(toAdd);

        await db.SaveChangesAsync();
    }

    // 🔧 FIX 2: everything below is now wrapped in this method (was floating loose).
    //           Deleted: the newCodes backfill, local docTypes/categories arrays,
    //           the errorCatalog filter, and the trailing ChangeTracker dedup block.
    /// <summary>Demo tenants/sites/users (password: Password123!) + fabricated batches — DEV ONLY.</summary>
    public static async Task SeedDemoDataAsync(AppDbContext db)
    {
        if (await db.Tenants.AnyAsync()) return; // idempotent guard

        var now = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var rng = new Random(20260625); // deterministic → reproducible data

        // ── Identity ───────────────────────────────────────────────
        var tenants = new[]
        {
            new Tenant { Id = AcmeId,   Name = "Acme Corp",  OrgDomain = "acme.com",   CreatedAt = now, IsActive = true },
            new Tenant { Id = GlobexId, Name = "Globex Inc", OrgDomain = "globex.com", CreatedAt = now, IsActive = true },
        };

        var sites = new[]
        {
            new Site { Id = AcmeMumbai,   TenantId = AcmeId,   Name = "Mumbai Plant",    Location = "Mumbai, IN",  CreatedAt = now, IsActive = true },
            new Site { Id = AcmeDelhi,    TenantId = AcmeId,   Name = "Delhi Warehouse", Location = "Delhi, IN",   CreatedAt = now, IsActive = true },
            new Site { Id = AcmeChennai,  TenantId = AcmeId,   Name = "Chennai Hub",     Location = "Chennai, IN", CreatedAt = now, IsActive = true },
            new Site { Id = AcmePune,     TenantId = AcmeId,   Name = "Pune Center",     Location = "Pune, IN",    CreatedAt = now, IsActive = true },
            new Site { Id = AcmeKolkata,  TenantId = AcmeId,   Name = "Kolkata Depot",   Location = "Kolkata, IN", CreatedAt = now, IsActive = true },
            new Site { Id = GlobexBerlin, TenantId = GlobexId, Name = "Berlin DC",       Location = "Berlin, DE",  CreatedAt = now, IsActive = true },
            new Site { Id = GlobexMunich, TenantId = GlobexId, Name = "Munich Plant",    Location = "Munich, DE",  CreatedAt = now, IsActive = true },
        };

        var users = new[]
        {
            new User { Id = DeveloperId, TenantId = null,     Email = "developer@platform.com", PasswordHash = hash, Role = "Developer", CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserA,   TenantId = AcmeId,   Email = "user.a@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AdminAcme,   TenantId = AcmeId,   Email = "admin@acme.com",    PasswordHash = hash, Role = "Admin",  CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserB,   TenantId = AcmeId,   Email = "user.b@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserD,   TenantId = AcmeId,   Email = "user.d@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserE,   TenantId = AcmeId,   Email = "user.e@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserF,   TenantId = AcmeId,   Email = "user.f@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserG,   TenantId = AcmeId,   Email = "user.g@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = GlobexUserC, TenantId = GlobexId, Email = "user.c@globex.com", PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AdminGlobex, TenantId = GlobexId, Email = "admin@globex.com",  PasswordHash = hash, Role = "Admin",  CreatedAt = now, IsActive = true },
        };

        var access = new[]
        {
            // Acme
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserA,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserA,   SiteId = AcmeDelhi,    GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeDelhi,    GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeChennai,  GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeKolkata,  GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserB,   SiteId = AcmeChennai,  GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserD,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserE,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserF,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserF,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserG,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserG,   SiteId = AcmeKolkata,  GrantedAt = now },
            // Globex
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = GlobexUserC, SiteId = GlobexBerlin, GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexBerlin, GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexMunich, GrantedAt = now },
        };

        // 🔧 FIX 3: catalogs already seeded by SeedCatalogsAsync → fetch them for FK references
        var docTypes = await db.Set<DocumentType>().ToArrayAsync();
        var categories = await db.Set<ItemCategory>().ToArrayAsync();

        string[] sources = { "S3_Bucket_Alpha", "SFTP_Beta", "API_Upload", "Legacy_Import", "Azure_Blob_Gamma" };
        string[] pipeline = { "Upload", "Validate", "Transform", "Load" };

        // ── Bulk transactional data ────────────────────────────────
        var transactions = new List<Transaction>();
        var files = new List<FileRecord>();
        var steps = new List<FileStepHistory>();
        var lineItems = new List<InvoiceLineItem>();
        var activity = new List<ActivityLog>();

        var siteTenant = new (Guid TenantId, Guid SiteId)[]
        {
            (AcmeId, AcmeMumbai), (AcmeId, AcmeDelhi), (AcmeId, AcmeChennai),
            (GlobexId, GlobexBerlin), (GlobexId, GlobexMunich), (AcmeId, AcmePune), (AcmeId, AcmeKolkata),
        };

        int batchSeq = 0;
        foreach (var (tenantId, siteId) in siteTenant)
        {
            int batchCount = rng.Next(22, 34);
            for (int b = 0; b < batchCount; b++)
            {
                batchSeq++;
                var submittedAt = now.AddDays(-rng.Next(0, 30))
                                     .AddHours(-rng.Next(0, 24))
                                     .AddMinutes(-rng.Next(0, 60));
                int fileCount = rng.Next(1, 9);
                int uploaded = 0, processing = 0, failed = 0, completed = 0;
                var lastUpdated = submittedAt;
                var txnId = Guid.NewGuid();

                for (int fi = 0; fi < fileCount; fi++)
                {
                    int r = rng.Next(100);
                    string status = r < 55 ? "Completed" : r < 75 ? "Failed" : r < 90 ? "Processing" : "Queued";
                    var docType = docTypes[rng.Next(docTypes.Length)];
                    string ext = docType.Category == "CSV" ? "csv" : "pdf";
                    var fileId = Guid.NewGuid();
                    var createdAt = submittedAt.AddMinutes(rng.Next(0, 10));
                    var fileUpdated = createdAt.AddMinutes(rng.Next(5, 180));
                    if (fileUpdated > lastUpdated) lastUpdated = fileUpdated;

                    string currentStep;
                    string? extractionStatus;
                    decimal? confidence;
                    var stepStart = createdAt;

                    if (status == "Completed")
                    {
                        foreach (var step in pipeline)
                        {
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = step, Status = "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2) });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        currentStep = "Load"; extractionStatus = "Done";
                        confidence = Math.Round((decimal)(0.80 + rng.NextDouble() * 0.19), 3);
                        completed++;
                    }
                    else if (status == "Failed")
                    {
                        int failAt = rng.Next(1, pipeline.Length);
                        var def = ErrorDefs[rng.Next(ErrorDefs.Length)];   // 🔧 errorDefs → ErrorDefs
                        for (int si = 0; si <= failAt; si++)
                        {
                            bool isFail = si == failAt;
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[si], Status = isFail ? "Failed" : "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2), ErrorCode = isFail ? def.Code : null, ErrorMessage = isFail ? def.Msg : null });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        currentStep = pipeline[failAt]; extractionStatus = "Failed";
                        confidence = Math.Round((decimal)(rng.NextDouble() * 0.4), 3);
                        failed++;
                        activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = "FILE_STATE_CHANGED", EntityType = "File", EntityId = fileId, EntityName = $"{docType.TypeName.ToLowerInvariant()}_{batchSeq}_{fi + 1}.{ext}", OldState = "Processing", NewState = "Failed", TriggeredBy = "system", CreatedAt = fileUpdated });
                    }
                    else if (status == "Processing")
                    {
                        int cur = rng.Next(1, pipeline.Length);
                        for (int si = 0; si < cur; si++)
                        {
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[si], Status = "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2) });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[cur], Status = "Processing", StartedAt = stepStart, CompletedAt = null });
                        currentStep = pipeline[cur]; extractionStatus = "Processing"; confidence = null;
                        processing++;
                    }
                    else // Queued
                    {
                        currentStep = "Upload"; extractionStatus = null; confidence = null;
                        uploaded++;
                    }

                    files.Add(new FileRecord
                    {
                        Id = fileId,
                        TenantId = tenantId,
                        SiteId = siteId,
                        TransactionId = txnId,
                        DocumentTypeId = docType.Id,
                        FileName = $"{docType.TypeName.ToLowerInvariant()}_{batchSeq}_{fi + 1}.{ext}",
                        FileType = docType.Category,
                        Status = status,
                        CurrentStep = currentStep,
                        FileSizeBytes = rng.Next(2_000, 5_000_000),
                        ExtractionStatus = extractionStatus,
                        ExtractionConfidence = confidence,
                        CreatedAt = createdAt,
                        LastUpdatedAt = fileUpdated
                    });

                    if (docType.TypeName == "Invoice" && status == "Completed")
                    {
                        int lines = rng.Next(2, 7);
                        for (int li = 1; li <= lines; li++)
                        {
                            ItemCategory? cat = rng.Next(100) < 15 ? null : categories[rng.Next(categories.Length)];
                            decimal qty = rng.Next(1, 200);
                            decimal unit = Math.Round((decimal)(rng.NextDouble() * 900 + 5), 2);
                            lineItems.Add(new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileId, TenantId = tenantId, SiteId = siteId, ItemCategoryId = cat?.Id, LineNumber = li, Description = $"{(cat?.CategoryName ?? "Uncategorized")} item {li}", Quantity = qty, UnitPrice = unit, LineTotal = Math.Round(qty * unit, 2), Confidence = Math.Round((decimal)(0.70 + rng.NextDouble() * 0.29), 3), IsValid = true, ExtractedAt = fileUpdated });
                        }
                    }
                }

                string state =
                    completed == fileCount ? "Completed" :
                    uploaded == fileCount ? "Queued" :
                    (processing > 0 || uploaded > 0) ? "Processing" : "Failed";

                bool terminal = state is "Completed" or "Failed";

                transactions.Add(new Transaction
                {
                    Id = txnId,
                    TenantId = tenantId,
                    SiteId = siteId,
                    State = state,
                    SourceSystem = sources[rng.Next(sources.Length)],
                    TotalFiles = fileCount,
                    UploadedCount = uploaded,
                    ProcessingCount = processing,
                    FailedCount = failed,
                    CompletedCount = completed,
                    SubmittedAt = submittedAt,
                    LastUpdatedAt = lastUpdated,
                    CompletedAt = terminal ? lastUpdated : null
                });

                activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = "BATCH_SUBMITTED", EntityType = "Batch", EntityId = txnId, EntityName = $"Batch {txnId.ToString()[..8]}", OldState = null, NewState = "Processing", TriggeredBy = "system", CreatedAt = submittedAt });
                if (terminal)
                    activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = state == "Completed" ? "BATCH_COMPLETED" : "BATCH_FAILED", EntityType = "Batch", EntityId = txnId, EntityName = $"Batch {txnId.ToString()[..8]}", OldState = "Processing", NewState = state, TriggeredBy = "system", CreatedAt = lastUpdated });
            }
        }

        // ── Persist (one round-trip) — catalogs are NOT re-added here ──
        db.AddRange(tenants);
        db.AddRange(sites);
        db.AddRange(users);
        db.AddRange(access);
        db.AddRange(transactions);
        db.AddRange(files);
        db.AddRange(steps);
        db.AddRange(lineItems);
        db.AddRange(activity);

        await db.SaveChangesAsync();
    }
}
