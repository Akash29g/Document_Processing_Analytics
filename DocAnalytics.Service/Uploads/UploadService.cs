using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Storage;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Uploads;

/// <summary>Default <see cref="IUploadService"/> implementation: batch creation, presigned upload URLs, duplicate handling, and pipeline hand-off.</summary>
public sealed class UploadService : IUploadService
{
    private const long MaxBytes = 15 * 1024 * 1024;   // 15 MB cap
    private static readonly HashSet<string> AllowedExtensions =
    new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg" };

    private static readonly Dictionary<string, string> MimeTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
        { ".pdf",  "application/pdf" },
        { ".jpg",  "image/jpeg" },
        { ".jpeg", "image/jpeg" }
        };
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;
    private readonly IFileStorage _storage;
    private readonly IExtractionQueue _queue;

    public UploadService(AppDbContext db, ICurrentUser me, IFileStorage storage, IExtractionQueue queue)
    {
        _db = db; _me = me; _storage = storage; _queue = queue;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<string?> GetDownloadUrlAsync(Guid fileId, CancellationToken ct = default)
    {
        // tenant-scoped by the global filter — other tenants get null → 404
        var file = await _db.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null || string.IsNullOrEmpty(file.StorageKey)) return null;
        return _storage.GetDownloadUrl(file.StorageKey, file.FileName, TimeSpan.FromMinutes(5));
    }


    /// <inheritdoc />
    // MODIFIED — attach file to an existing batch (no new Transaction)
    public async Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(req.FileName);
        if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Only PDF, JPG, and JPEG files are supported.");
        var contentType = MimeTypes[ext];
        var fileTypeName = ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase) ? "PDF" : "JPEG";
        if (req.SizeBytes is <= 0 or > MaxBytes)
            throw new InvalidOperationException("File is empty or exceeds the 15 MB limit.");

        // verify the batch exists and belongs to this tenant/site (unchanged)
        var batchExists = await _db.Set<Transaction>()
            .AnyAsync(t => t.Id == req.BatchId && t.TenantId == _me.TenantId && t.SiteId == _me.SiteId, ct);
        if (!batchExists)
            throw new InvalidOperationException("Batch not found.");

        var invoiceTypeId = await _db.Set<DocumentType>()
            .Where(d => d.TypeName == "Invoice")
            .Select(d => (Guid?)d.Id)
            .FirstOrDefaultAsync(ct);

        // ── NEW: friendly S3 path — tenant/site NAMES instead of GUIDs ──
        var tenantName = await _db.Tenants.Where(t => t.Id == _me.TenantId).Select(t => t.Name).FirstAsync(ct);
        var siteName = await _db.Sites.Where(s => s.Id == _me.SiteId).Select(s => s.Name).FirstAsync(ct);

        var now = DateTime.UtcNow;
        var fileName = req.FileName;

        // ── NEW: duplicate check — same file name uploaded today for this site ──
        var today = now.Date;
        var existing = await _db.Files.FirstOrDefaultAsync(f =>
            f.FileName == fileName && f.CreatedAt >= today && f.CreatedAt < today.AddDays(1), ct);

        if (existing is not null)
        {
            switch (req.OnDuplicate?.ToLowerInvariant())
            {
                case "replace":
                    _db.Files.Remove(existing);   // cascades: steps, line items, header
                    break;
                case "rename":
                    fileName = await MakeUniqueNameAsync(fileName, today, ct);
                    break;
                default:
                    throw new DuplicateFileException(fileName);   // → controller returns 409
            }
        }

        var fileId = Guid.NewGuid();
        var key = _storage.BuildKey(tenantName, siteName, now, fileName);   // 👈 NEW signature

        var file = new FileRecord
        {
            Id = fileId,
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = req.BatchId,
            DocumentTypeId = invoiceTypeId,
            FileName = fileName,              // 👈 NOTE: fileName (may be renamed), not req.FileName
            FileType = fileTypeName,
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

        var url = await _storage.GetPresignedPutUrlAsync(key, contentType, TimeSpan.FromMinutes(5), ct);
        return new UploadUrlResponse { FileId = fileId, UploadUrl = url };
    }

    /// <inheritdoc />
    // Called when the user skips a duplicate: the batch now expects one fewer file.
    public async Task<bool> ShrinkBatchAsync(Guid batchId, CancellationToken ct = default)
    {
        var txn = await _db.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == batchId, ct);
        if (txn is null) return false;

        txn.TotalFiles = Math.Max(0, txn.TotalFiles - 1);
        txn.LastUpdatedAt = DateTime.UtcNow;

        // if every file was skipped, remove the empty batch entirely
        if (txn.TotalFiles == 0 && !await _db.Files.AnyAsync(f => f.TransactionId == txn.Id, ct))
        {
            _db.Remove(txn);
        }
        else
        {
            // maybe the remaining files already settled → finalize now
            var settled = txn.CompletedCount + txn.FailedCount;
            if (settled >= txn.TotalFiles)
            {
                txn.State = txn.FailedCount > 0 ? "Failed" : "Completed";
                txn.CompletedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBatchAsync(Guid batchId, CancellationToken ct = default)
    {
        var txn = await _db.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == batchId, ct);
        if (txn is null) return false;

        var files = await _db.Files.Where(f => f.TransactionId == batchId).ToListAsync(ct);

        // 1) S3 objects first (best-effort — don't leave DB rows if S3 fails midway)
        foreach (var f in files.Where(f => !string.IsNullOrEmpty(f.StorageKey)))
            await _storage.DeleteAsync(f.StorageKey!, ct);

        // 2) DB: files cascade to headers/line-items/errors; then the batch itself
        // 2) DB: files cascade to headers/line-items/errors; then the batch itself
        _db.Files.RemoveRange(files);
        _db.Remove(txn);

        // ── audit trail: write BEFORE SaveChanges so it commits in the same transaction ──
        _db.Add(new DocAnalytics.Domain.Entities.ActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = txn.TenantId,
            SiteId = txn.SiteId,
            EventType = "BATCH_DELETED",
            EntityType = "Batch",
            EntityId = txn.Id,
            EntityName = txn.SourceSystem,
            OldState = txn.State,
            NewState = "Deleted",
            TriggeredBy = _me.UserId.ToString(),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        return true;

    }




    /// <inheritdoc />
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

    private async Task<string> MakeUniqueNameAsync(string name, DateTime today, CancellationToken ct)
    {
        var stem = Path.GetFileNameWithoutExtension(name);
        var ext = Path.GetExtension(name);
        for (var i = 2; ; i++)
        {
            var candidate = $"{stem} ({i}){ext}";
            var taken = await _db.Files.AnyAsync(f =>
                f.FileName == candidate && f.CreatedAt >= today && f.CreatedAt < today.AddDays(1), ct);
            if (!taken) return candidate;
        }
    }

}

/// <summary>Thrown when a file with the same name has already been uploaded today for the current site.</summary>
public sealed class DuplicateFileException(string fileName)
    : Exception($"\"{fileName}\" was already uploaded today.");
