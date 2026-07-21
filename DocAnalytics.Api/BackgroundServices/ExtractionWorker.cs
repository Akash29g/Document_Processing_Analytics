using System.Diagnostics.CodeAnalysis;
using DocAnalytics.Api.Common;
using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Realtime;
using DocAnalytics.Service.Storage;
using Microsoft.EntityFrameworkCore;


namespace DocAnalytics.Api.BackgroundServices;

/// <summary>
/// Hosted worker that drains the extraction queue and runs the invoice pipeline per file:
/// download → malware/format security gates → Bedrock extraction → validation → persist header/line items,
/// updating file/batch state and broadcasting real-time notifications throughout.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ExtractionWorker : BackgroundService
{
    private readonly IExtractionQueue _queue;
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<ExtractionWorker> _logger;

    /// <summary>Creates the worker with the shared queue, a scope factory, and a logger.</summary>
    /// <param name="queue">The extraction queue to consume.</param>
    /// <param name="scopes">Factory used to create a DI scope per file.</param>
    /// <param name="logger">The logger.</param>
    public ExtractionWorker(IExtractionQueue queue, IServiceScopeFactory scopes, ILogger<ExtractionWorker> logger)
    {
        _queue = queue; _scopes = scopes; _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var fileId in _queue.DequeueAllAsync(stoppingToken))
        {
            try { await ProcessAsync(fileId, stoppingToken); }
            catch (Exception ex) { _logger.LogError(ex, "Extraction failed for file {FileId}", fileId); }
        }
    }

    /// <summary>Runs the full extraction pipeline for a single file within its own DI scope.</summary>
    private async Task ProcessAsync(Guid fileId, CancellationToken ct)
    {
        using var scope = _scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var extractor = scope.ServiceProvider.GetRequiredService<IInvoiceExtractor>();
        var validator = scope.ServiceProvider.GetRequiredService<IInvoiceValidator>();
        var storage = scope.ServiceProvider.GetRequiredService<IFileStorage>();
        var notifier = scope.ServiceProvider.GetRequiredService<IPipelineNotifier>();

        // ⚠️ GOTCHA #1: no JWT here → read the file WITHOUT the tenant filter, then set context.
        var file = await db.Files.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null) return;

        var me = (CurrentUser)scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        me.Set(Guid.Empty, file.TenantId, file.SiteId, "system");   // scope all further queries

        var txn = await db.Transactions.FirstAsync(t => t.Id == file.TransactionId, ct);
        var now = DateTime.UtcNow;

        // → Processing
        file.Status = "Processing"; file.CurrentStep = "Extract"; file.ExtractionStatus = "Processing";
        file.LastUpdatedAt = now;
        txn.UploadedCount = Math.Max(0, txn.UploadedCount - 1);
        txn.ProcessingCount += 1;
        db.Add(new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = file.Id,
            DocumentTypeId = file.DocumentTypeId,
            StepName = "Extract",
            Status = "Processing",
            StartedAt = now
        });
        await db.SaveChangesAsync(ct);
        await notifier.NotifyFileStateChangedAsync(file.SiteId,
            new FileStateChangedNotification(file.Id, file.FileName, "Queued", "Processing", "Extract", now), ct);

        try
        {
            var bytes = await storage.DownloadAsync(file.StorageKey!, ct);
            var result = await extractor.ExtractAsync(bytes, ct);
            var v = validator.Validate(result);

            // ── SECURITY GATE 1: GuardDuty malware verdict (tag is written async, poll briefly) ──
            string? scan = null;
            for (var attempt = 0; attempt < 12; attempt++)          // up to ~60s
            {
                scan = await storage.GetMalwareScanStatusAsync(file.StorageKey!, ct);
                if (scan is not null) break;
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
            if (scan == "THREATS_FOUND")
            {
                await storage.DeleteAsync(file.StorageKey!, ct);     // never servable again
                file.StorageKey = null;                              // download-url → 404
                await FailFileAsync(db, notifier, file, txn, now,
                    "ERR_MALWARE_DETECTED", "Malware detected in uploaded file; the file has been removed.", ct);
                return;
            }

            // ── SECURITY GATE 2: magic bytes — real PDFs start with %PDF- ──
            if (bytes.Length < 5 || bytes[0] != 0x25 || bytes[1] != 0x50 ||
                bytes[2] != 0x44 || bytes[3] != 0x46 || bytes[4] != 0x2D)
            {
                await storage.DeleteAsync(file.StorageKey!, ct);
                file.StorageKey = null;
                await FailFileAsync(db, notifier, file, txn, now,
                    "ERR_INVALID_FILETYPE", "File content is not a valid PDF (extension spoofing suspected).", ct);
                return;
            }



            // ⚠️ GOTCHA #2: idempotent → clear existing line items first
            var old = await db.InvoiceLineItems.Where(i => i.FileId == file.Id).ToListAsync(ct);
            db.RemoveRange(old);

            var cats = await db.Set<ItemCategory>().ToListAsync(ct);   // tracked — we may add to it

            Guid? ResolveCategory(string? name)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                var clean = name.Trim();

                // 1) try to match an existing category
                var m = cats.FirstOrDefault(c =>
                    string.Equals(c.CategoryName, clean, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.CategoryCode, clean, StringComparison.OrdinalIgnoreCase) ||
                    clean.Contains(c.CategoryName, StringComparison.OrdinalIgnoreCase));
                if (m is not null) return m.Id;

                // 2) not found → create it on the fly
                var created = new ItemCategory
                {
                    Id = Guid.NewGuid(),
                    CategoryCode = clean.ToUpperInvariant().Replace(' ', '_'),
                    CategoryName = clean,
                };
                db.Add(created);
                cats.Add(created);   // reuse within this same file
                return created.Id;
            }


            foreach (var li in result.LineItems)
                db.Add(new InvoiceLineItem
                {
                    Id = Guid.NewGuid(),
                    FileId = file.Id,
                    TenantId = file.TenantId,
                    SiteId = file.SiteId,
                    ItemCategoryId = ResolveCategory(li.Category),
                    LineNumber = li.LineNumber,
                    Description = li.Description,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                    LineTotal = li.LineTotal,
                    Confidence = v.Confidence,
                    IsValid = v.IsValid,
                    ExtractedAt = DateTime.UtcNow,
                });

            // idempotent header upsert (same pattern as line items — gotcha #2)
            var oldHeader = await db.Set<InvoiceHeader>().Where(h => h.FileId == file.Id).ToListAsync(ct);
            db.RemoveRange(oldHeader);
            db.Add(new InvoiceHeader
            {
                Id = Guid.NewGuid(),
                FileId = file.Id,
                TenantId = file.TenantId,
                SiteId = file.SiteId,
                InvoiceNumber = result.InvoiceNumber,
                InvoiceDate = result.InvoiceDate,
                Seller = result.Seller,
                Buyer = result.Client,
                Currency = result.Currency,
                Subtotal = result.Subtotal,
                Discount = result.Discount,
                Tax = result.Tax,
                Shipping = result.Shipping,
                Total = result.Total,
                ExtractedAt = DateTime.UtcNow,
            });


            var done = DateTime.UtcNow;
            bool failed = !v.IsValid;

            file.Status = failed ? "Failed" : "Completed";
            file.CurrentStep = failed ? "Extract" : "Load";
            file.ExtractionStatus = failed ? "Failed" : "Done";
            file.ExtractionConfidence = v.Confidence;
            file.LastUpdatedAt = done;

            db.Add(new FileStepHistory
            {
                Id = Guid.NewGuid(),
                FileId = file.Id,
                DocumentTypeId = file.DocumentTypeId,
                StepName = "Extract",
                Status = failed ? "Failed" : "Success",
                StartedAt = now,
                CompletedAt = done,
                ErrorCode = failed ? v.ErrorCode : null,
                ErrorMessage = failed ? "Extraction confidence too low." : null
            });

            txn.ProcessingCount = Math.Max(0, txn.ProcessingCount - 1);
            if (failed) txn.FailedCount += 1; else txn.CompletedCount += 1;
            RecomputeState(txn, done);

            db.Add(new ActivityLog
            {
                Id = Guid.NewGuid(),
                TenantId = file.TenantId,
                SiteId = file.SiteId,
                EventType = "FILE_STATE_CHANGED",
                EntityType = "File",
                EntityId = file.Id,
                EntityName = file.FileName,
                OldState = "Processing",
                NewState = file.Status,
                TriggeredBy = "system",
                CreatedAt = done
            });

            await db.SaveChangesAsync(ct);
            await notifier.NotifyFileStateChangedAsync(file.SiteId,
                new FileStateChangedNotification(file.Id, file.FileName, "Processing", file.Status, "Extract", done), ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bedrock/extraction error for {FileId}", file.Id);
            var done = DateTime.UtcNow;
            file.Status = "Failed"; file.CurrentStep = "Extract"; file.ExtractionStatus = "Failed";
            file.LastUpdatedAt = done;
            db.Add(new FileStepHistory
            {
                Id = Guid.NewGuid(),
                FileId = file.Id,
                DocumentTypeId = file.DocumentTypeId,
                StepName = "Extract",
                Status = "Failed",
                StartedAt = now,
                CompletedAt = done,
                ErrorCode = "ERR_EXTRACTION_FAILED",
                ErrorMessage = ex.Message
            });
            txn.ProcessingCount = Math.Max(0, txn.ProcessingCount - 1);
            txn.FailedCount += 1;
            RecomputeState(txn, done);
            await db.SaveChangesAsync(ct);
        }
    }

    /// <summary>Marks a file (and its parent batch counters) as failed with the given error, logs it, and broadcasts the change.</summary>
    private static async Task FailFileAsync(
    AppDbContext db, IPipelineNotifier notifier,
    FileRecord file, Transaction txn, DateTime startedAt,
    string errorCode, string errorMessage, CancellationToken ct)
    {
        var done = DateTime.UtcNow;
        file.Status = "Failed"; file.CurrentStep = "Extract"; file.ExtractionStatus = "Failed";
        file.LastUpdatedAt = done;

        db.Add(new FileStepHistory
        {
            Id = Guid.NewGuid(),
            FileId = file.Id,
            DocumentTypeId = file.DocumentTypeId,
            StepName = "Extract",
            Status = "Failed",
            StartedAt = startedAt,
            CompletedAt = done,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        });

        txn.ProcessingCount = Math.Max(0, txn.ProcessingCount - 1);
        txn.FailedCount += 1;
        RecomputeState(txn, done);

        db.Add(new ActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = file.TenantId,
            SiteId = file.SiteId,
            EventType = "FILE_STATE_CHANGED",
            EntityType = "File",
            EntityId = file.Id,
            EntityName = file.FileName,
            OldState = "Processing",
            NewState = "Failed",
            TriggeredBy = "system",
            CreatedAt = done
        });

        await db.SaveChangesAsync(ct);
        await notifier.NotifyFileStateChangedAsync(file.SiteId,
            new FileStateChangedNotification(file.Id, file.FileName, "Processing", "Failed", "Extract", done), ct);
    }


    /// <summary>Recomputes the batch state (DT-1): any failure marks the batch Failed, but only once every file is settled.</summary>
    // DT-1 preserved: any file fails → batch Failed. But only finalize once ALL files are settled.
    private static void RecomputeState(Transaction t, DateTime at)
    {
        var settled = t.CompletedCount + t.FailedCount;
        var allDone = settled >= t.TotalFiles;

        t.State = allDone
            ? (t.FailedCount > 0 ? "Failed" : "Completed")
            : "Processing";

        t.LastUpdatedAt = at;
        t.CompletedAt = allDone ? at : null;
    }

}
