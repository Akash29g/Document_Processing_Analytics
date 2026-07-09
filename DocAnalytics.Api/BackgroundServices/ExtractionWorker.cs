using DocAnalytics.Api.Common;
using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Realtime;
using DocAnalytics.Service.Storage;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Api.BackgroundServices;

public sealed class ExtractionWorker : BackgroundService
{
    private readonly IExtractionQueue _queue;
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<ExtractionWorker> _logger;

    public ExtractionWorker(IExtractionQueue queue, IServiceScopeFactory scopes, ILogger<ExtractionWorker> logger)
    {
        _queue = queue; _scopes = scopes; _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var fileId in _queue.DequeueAllAsync(stoppingToken))
        {
            try { await ProcessAsync(fileId, stoppingToken); }
            catch (Exception ex) { _logger.LogError(ex, "Extraction failed for file {FileId}", fileId); }
        }
    }

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

            // ⚠️ GOTCHA #2: idempotent → clear existing line items first
            var old = await db.InvoiceLineItems.Where(i => i.FileId == file.Id).ToListAsync(ct);
            db.RemoveRange(old);

            var catByCode = await db.Set<ItemCategory>().AsNoTracking()
                .ToDictionaryAsync(c => c.CategoryCode, c => c.Id, ct);

            foreach (var li in result.LineItems)
                db.Add(new InvoiceLineItem
                {
                    Id = Guid.NewGuid(),
                    FileId = file.Id,
                    TenantId = file.TenantId,
                    SiteId = file.SiteId,
                    ItemCategoryId = null,   // Nova doesn't categorize; leave null (LEFT-join safe)
                    LineNumber = li.LineNumber,
                    Description = li.Description,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                    LineTotal = li.LineTotal,
                    Confidence = v.Confidence,
                    IsValid = v.IsValid,
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
