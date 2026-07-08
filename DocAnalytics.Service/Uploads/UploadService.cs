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

    public async Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default)
    {
        if (!req.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only PDF invoices are supported.");
        if (req.SizeBytes is <= 0 or > MaxBytes)
            throw new InvalidOperationException("File is empty or exceeds the 15 MB limit.");

        // Invoice document type (global catalog — not tenant scoped)
        var invoiceTypeId = await _db.Set<DocumentType>()
            .Where(d => d.TypeName == "Invoice")
            .Select(d => (Guid?)d.Id)
            .FirstOrDefaultAsync(ct);

        var now = DateTime.UtcNow;
        var fileId = Guid.NewGuid();
        var key = _storage.BuildKey(_me.TenantId, _me.SiteId, fileId);

        // one batch per upload, tagged so the source filter separates it from seed data
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            State = "Processing",
            SourceSystem = "Manual_Upload",     // 👈 the tag we reserved
            TotalFiles = 1,
            UploadedCount = 1,
            ProcessingCount = 0,
            FailedCount = 0,
            CompletedCount = 0,
            SubmittedAt = now,
            LastUpdatedAt = now,
            CompletedAt = null,
        };

        var file = new FileRecord
        {
            Id = fileId,
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = txn.Id,
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

        _db.Add(txn);
        _db.Add(file);
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

        var url = await _storage.GetPresignedPutUrlAsync(key, "application/pdf", TimeSpan.FromMinutes(5), ct);
        return new UploadUrlResponse { FileId = fileId, UploadUrl = url };
    }

    public async Task<bool> CompleteAsync(Guid fileId, CancellationToken ct = default)
    {
        var exists = await _db.Files.AsNoTracking().AnyAsync(f => f.Id == fileId, ct);
        if (!exists) return false;
        await _queue.EnqueueAsync(fileId, ct);   // hand off to the worker
        return true;
    }
}
