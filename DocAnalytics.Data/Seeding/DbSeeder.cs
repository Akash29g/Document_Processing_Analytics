using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data.Seeding;

public static class DbSeeder
{
    // ─────────────────────────────────────────────────────────────
    // FIXED IDs — stable across resets so tokens & site_id never go stale.
    // Login: Password123!  for every user below.
    //

    //  TENANTS
    //   Acme   : 11111111-1111-1111-1111-111111111111
    //   Globex : 22222222-2222-2222-2222-222222222222

    //  SITES (paste these into X-Site-Id)
    //   Acme/Mumbai  : a1111111-1111-1111-1111-111111111111
    //   Acme/Delhi   : a2222222-2222-2222-2222-222222222222
    //   Acme/Chennai : a3333333-3333-3333-3333-333333333333
    //   Acme/Pune    : a4444444-4444-4444-4444-444444444444
    //   Acme/Kolkata : a5555555-5555-5555-5555-555555555555
    //   Globex/Berlin: b1111111-1111-1111-1111-111111111111
    //   Globex/Munich: b2222222-2222-2222-2222-222222222222

    //  USERS  (password: Password123!)
    //   user.a@acme.com    (Viewer, Acme   — Mumbai + Delhi)
    //   admin@acme.com     (Admin,  Acme   — all 5 Acme sites: Mumbai, Delhi, Chennai, Pune, Kolkata)
    //   user.b@acme.com    (Viewer, Acme   — Chennai only)
    //   user.d@acme.com    (Viewer, Acme   — Mumbai only)
    //   user.e@acme.com    (Viewer, Acme   — Mumbai only)
    //   user.f@acme.com    (Viewer, Acme   — Mumbai + Pune)
    //   user.g@acme.com    (Viewer, Acme   — Pune + Kolkata)
    //   user.c@globex.com  (Viewer, Globex — Berlin only)
    //   admin@globex.com   (Admin,  Globex — both Globex sites: Berlin, Munich)
 
    //  RECIPIENTS PER SITE (S-4 alert dropdown):
    //   Mumbai  → user.a, admin@acme, user.d, user.e, user.f  (5)
    //   Pune    → admin@acme, user.f, user.g                  (3)
    //   Kolkata → admin@acme, user.g                          (2)
    //   Delhi   → user.a, admin@acme                          (2)
    //   Chennai → admin@acme, user.b                          (2)


    // ─────────────────────────────────────────────────────────────

    private static readonly Guid AcmeId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid GlobexId = new("22222222-2222-2222-2222-222222222222");

    private static readonly Guid AcmeMumbai = new("a1111111-1111-1111-1111-111111111111");
    private static readonly Guid AcmeDelhi = new("a2222222-2222-2222-2222-222222222222");
    private static readonly Guid AcmeChennai = new("a3333333-3333-3333-3333-333333333333");
    private static readonly Guid AcmePune = new("a4444444-4444-4444-4444-444444444444");
    private static readonly Guid AcmeKolkata = new("a5555555-5555-5555-5555-555555555555");
    private static readonly Guid GlobexBerlin = new("b1111111-1111-1111-1111-111111111111");
    private static readonly Guid GlobexMunich = new("b2222222-2222-2222-2222-222222222222");

    private static readonly Guid AcmeUserA = new("c1111111-1111-1111-1111-111111111111");
    private static readonly Guid AcmeUserB = new("c4444444-4444-4444-4444-444444444444");
    private static readonly Guid AcmeUserD = new("c6666666-6666-6666-6666-666666666666");
    private static readonly Guid AcmeUserE = new("c7777777-7777-7777-7777-777777777777");
    private static readonly Guid AcmeUserF = new("c8888888-8888-8888-8888-888888888888");
    private static readonly Guid AcmeUserG = new("c9999999-9999-9999-9999-999999999999");
    private static readonly Guid AdminAcme = new("c2222222-2222-2222-2222-222222222222");
    private static readonly Guid GlobexUserC = new("c3333333-3333-3333-3333-333333333333");
    private static readonly Guid AdminGlobex = new("c5555555-5555-5555-5555-555555555555");

    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        if (await db.Tenants.AnyAsync()) return; // idempotent guard

        var now = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var rng = new Random(20260625); // deterministic → reproducible data

        // ── Identity ───────────────────────────────────────────────
        var tenants = new[]
        {
            new Tenant { Id = AcmeId,   Name = "Acme Corp",  CreatedAt = now, IsActive = true },
            new Tenant { Id = GlobexId, Name = "Globex Inc", CreatedAt = now, IsActive = true },
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
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserA,   SiteId = AcmeDelhi,    GrantedAt = now },  // user.a: Mumbai + Delhi
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeDelhi,    GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeChennai,  GrantedAt = now },  // admin@acme: all 5
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeKolkata,  GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserB,   SiteId = AcmeChennai,  GrantedAt = now },  // user.b: Chennai only
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserD,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserE,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserF,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserF,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserG,   SiteId = AcmePune,     GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserG,   SiteId = AcmeKolkata,  GrantedAt = now },

            // Globex
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = GlobexUserC, SiteId = GlobexBerlin, GrantedAt = now },  // user.c: Berlin only
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexBerlin, GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexMunich, GrantedAt = now },  // admin@globex: both

        };

        // ── Global catalogs ────────────────────────────────────────
        var docTypes = new[]
        {
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Invoice",       Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Manifest",      Category = "CSV", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "PurchaseOrder", Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Receipt",       Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "PackingSlip",   Category = "CSV", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "BillOfLading",  Category = "PDF", IsActive = true, CreatedAt = now },
        };

        var categories = new[]
        {
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "GOODS",    CategoryName = "Goods",    IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SERVICES", CategoryName = "Services", IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SHIPPING", CategoryName = "Shipping", IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "TAX",      CategoryName = "Tax",      IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "MISC",     CategoryName = "Misc",     IsActive = true, CreatedAt = now },
        };

        var errorDefs = new (string Code, string Desc, string Remediation, string Msg)[]
        {
            ("ERR_BAD_SCHEMA",        "CSV column headers do not match the expected schema.", "Check your column headers against the template.", "Unexpected column 'qty2'."),
            ("ERR_TIMEOUT",           "Processing step exceeded the allotted time.",          "Retry; if it persists, reduce the file size.",    "Step timed out after 300s."),
            ("ERR_CORRUPT_FILE",      "The uploaded file is corrupt or unreadable.",          "Re-export the document and upload again.",        "Unable to open file: unexpected EOF."),
            ("ERR_OCR_LOW_CONFIDENCE","Extraction confidence below the accepted threshold.",  "Upload a higher-resolution scan.",                "OCR confidence 0.42 < 0.70 threshold."),
            ("ERR_MISSING_FIELD",     "A required field was missing from the document.",       "Ensure all mandatory fields are present.",        "Required field 'invoice_total' not found."),
            ("ERR_UNSUPPORTED_FORMAT","The file format is not supported.",                     "Convert the file to PDF or CSV.",                 "Format '.docx' is not supported."),
            ("ERR_DUPLICATE",         "A document with the same hash already exists.",         "Remove the duplicate before re-submitting.",      "Duplicate of an existing file."),
            ("ERR_AUTH_UPSTREAM",     "Authentication with an upstream service failed.",       "Renew upstream credentials and retry.",           "Upstream returned 401 Unauthorized."),
        };
        var errorCatalog = errorDefs
            .Select(e => new ErrorCatalog { Id = Guid.NewGuid(), ErrorCode = e.Code, Description = e.Desc, RemediationMsg = e.Remediation, CreatedAt = now, UpdatedAt = now })
            .ToArray();

        string[] sources = { "S3_Bucket_Alpha", "SFTP_Beta", "API_Upload", "Manual_Upload", "Azure_Blob_Gamma" };
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
            int batchCount = rng.Next(22, 34); // ~22–33 batches per site
            for (int b = 0; b < batchCount; b++)
            {
                batchSeq++;
                var submittedAt = now.AddDays(-rng.Next(0, 30))
                                     .AddHours(-rng.Next(0, 24))
                                     .AddMinutes(-rng.Next(0, 60));

                int fileCount = rng.Next(1, 9); // 1–8 files
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
                        int failAt = rng.Next(1, pipeline.Length); // fail at Validate/Transform/Load
                        var def = errorDefs[rng.Next(errorDefs.Length)];
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

                    // Line items for completed invoices
                    if (docType.TypeName == "Invoice" && status == "Completed")
                    {
                        int lines = rng.Next(2, 7);
                        for (int li = 1; li <= lines; li++)
                        {
                            // ~15% of lines left uncategorized → exercises the LEFT-join null-category path
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

        // ── Persist (one round-trip) ───────────────────────────────
        db.AddRange(tenants);
        db.AddRange(sites);
        db.AddRange(users);
        db.AddRange(access);
        db.AddRange(docTypes);
        db.AddRange(categories);
        db.AddRange(errorCatalog);
        db.AddRange(transactions);
        db.AddRange(files);
        db.AddRange(steps);
        db.AddRange(lineItems);
        db.AddRange(activity);
        await db.SaveChangesAsync();
    }
}
