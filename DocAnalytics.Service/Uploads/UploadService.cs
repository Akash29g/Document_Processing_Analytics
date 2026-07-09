using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Storage;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Uploads;

public sealed class UploadService : IUploadService
{
    private const long MaxBytes = 15 * 1024 * 1024;   // 15 MB cap
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;
    private readonly IFileStorage _storage;
    private readonly IExtractionQueue _queue;

    public UploadService(AppDbContext db, ICurrentUser me, IFileStorage storage, IExtractionQueue queue)
    {
        _db = db; _me = me; _storage = storage; _queue = queue;
    }

    // 👇 NEW — open ONE batch for the whole upload
    public async Task<CreateBatchResponse> CreateBatchAsync(CreateBatchRequest req, CancellationToken ct = default)
    {
        if (req.FileCount <= 0)
            throw new InvalidOperationException("No files provided.");

        var now = DateTime.UtcNow;
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            State = "Processing",
            SourceSystem = "Manual_Upload",
            TotalFiles = req.FileCount,      // 👈 N, not 1
            UploadedCount = 0,
            ProcessingCount = 0,
            FailedCount = 0,
            CompletedCount = 0,
            SubmittedAt = now,
            LastUpdatedAt = now,
            CompletedAt = null,
        };

        _db.Add(txn);
        _db.Add(new DocAnalytics.Domain.Entities.ActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            EventType = "BATCH_SUBMITTED",
            EntityType = "Batch",
            EntityId = txn.Id,
            EntityName = $"Batch {txn.Id.ToString()[..8]}",
            OldState = null,
            NewState = "Processing",
            TriggeredBy = "user",
            CreatedAt = now,
        });
        await _db.SaveChangesAsync(ct);

        return new CreateBatchResponse { BatchId = txn.Id };
    }

    // MODIFIED — attach file to an existing batch (no new Transaction)
    public async Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default)
    {
        if (!req.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only PDF invoices are supported.");
        if (req.SizeBytes is <= 0 or > MaxBytes)
            throw new InvalidOperationException("File is empty or exceeds the 15 MB limit.");

        // verify the batch exists and belongs to this tenant/site
        var batchExists = await _db.Set<Transaction>()
            .AnyAsync(t => t.Id == req.BatchId && t.TenantId == _me.TenantId && t.SiteId == _me.SiteId, ct);
        if (!batchExists)
            throw new InvalidOperationException("Batch not found.");

        var invoiceTypeId = await _db.Set<DocumentType>()
            .Where(d => d.TypeName == "Invoice")
            .Select(d => (Guid?)d.Id)
            .FirstOrDefaultAsync(ct);

        var now = DateTime.UtcNow;
        var fileId = Guid.NewGuid();
        var key = _storage.BuildKey(_me.TenantId, _me.SiteId, fileId);

        var file = new FileRecord
        {
            Id = fileId,
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = req.BatchId,     // 👈 shared batch
            DocumentTypeId = invoiceTypeId,
            FileName = req.FileName,
            FileType = "PDF",
            Status = "Queued",
            CurrentStep = "Upload",
            FileSizeBytes = req.SizeBytes,
            ExtractionStatus = "Pending",
            ExtractionConfidence = null,
            StorageKey = key,
            CreatedAt = now,
            LastUpdatedAt = now,
        };

        _db.Add(file);
        await _db.SaveChangesAsync(ct);

        var url = await _storage.GetPresignedPutUrlAsync(key, "application/pdf", TimeSpan.FromMinutes(5), ct);
        return new UploadUrlResponse { FileId = fileId, UploadUrl = url };
    }

    // MODIFIED — bump batch UploadedCount, then enqueue
    public async Task<bool> CompleteAsync(Guid fileId, CancellationToken ct = default)
    {
        var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null) return false;

        var batch = await _db.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == file.TransactionId, ct);
        if (batch is not null)
        {
            batch.UploadedCount += 1;
            batch.LastUpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        await _queue.EnqueueAsync(fileId, ct);   // hand off to the worker
        return true;
    }
}
