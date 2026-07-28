using System.Text;
using DocAnalytics.Data;
using DocAnalytics.Domain.Common;    // ICurrentUser
using DocAnalytics.Domain.Entities;  // ActivityLog, Transaction
using ActivityLogEntry = DocAnalytics.Domain.Entities.ActivityLog;  // disambiguate from Service.ActivityLog namespace
using DocAnalytics.Service.Extraction;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Files;

/// <summary>Default <see cref="IFileDetailsService"/> implementation.</summary>
public sealed class FileDetailsService : IFileDetailsService
{
    private readonly AppDbContext _db;
    private readonly IExtractionQueue _queue;
    private readonly ICurrentUser _currentUser;

    public FileDetailsService(
        AppDbContext db,
        IExtractionQueue queue,
        ICurrentUser currentUser)
    {
        _db = db;
        _queue = queue;
        _currentUser = currentUser;
    }

    // ── GET /api/v1/files/{id}/details ──────────────────────────────────────
    public async Task<FileDetailDto?> GetFileDetailsAsync(
        Guid fileId, CancellationToken ct = default)
    {
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;

        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        var codes = steps
            .Where(s => s.ErrorCode != null)
            .Select(s => s.ErrorCode!)
            .Distinct()
            .ToList();

        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        return new FileDetailDto
        {
            FileInfo = new FileInfoDto
            {
                Id = file.Id,
                Name = file.FileName,
                CurrentStatus = file.Status,
                CurrentStep = file.CurrentStep,
            },
            History = steps.Select(s => new StepHistoryDto
            {
                Step = s.StepName,
                Status = s.Status,
                Ts = s.StartedAt ?? s.CompletedAt,
                Error = s.ErrorCode is null ? null : new StepErrorDto
                {
                    Code = s.ErrorCode,
                    Message = s.ErrorMessage,
                    SuggestedFix = remediation.TryGetValue(s.ErrorCode, out var fix) ? fix : null,
                },
            }).ToList(),
        };
    }

    // ── GET /api/v1/files/{id}/logs ──────────────────────────────────────────
    public async Task<FileLogDto?> GetFileLogsAsync(
        Guid fileId, CancellationToken ct = default)
    {
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;

        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        var codes = steps
            .Where(s => s.ErrorCode != null)
            .Select(s => s.ErrorCode!)
            .Distinct()
            .ToList();

        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        var sb = new StringBuilder();
        sb.AppendLine("=== Document Processing — File Step Log ===");
        sb.AppendLine($"File          : {file.FileName}");
        sb.AppendLine($"File Id       : {file.Id}");
        sb.AppendLine($"Current Status: {file.Status}");
        sb.AppendLine($"Current Step  : {file.CurrentStep}");
        sb.AppendLine($"Generated     : {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}");
        sb.AppendLine(new string('-', 60));

        foreach (var s in steps)
        {
            var ts = (s.StartedAt ?? s.CompletedAt)?.ToString("yyyy-MM-ddTHH:mm:ssZ")
                     ?? "(no timestamp)";
            sb.AppendLine($"[{ts}] {s.StepName,-12} {s.Status}");
            if (s.ErrorCode is not null)
            {
                sb.AppendLine($"    Error      : {s.ErrorCode} — {s.ErrorMessage}");
                if (remediation.TryGetValue(s.ErrorCode, out var fix) && !string.IsNullOrWhiteSpace(fix))
                    sb.AppendLine($"    Suggested  : {fix}");
            }
        }

        return new FileLogDto
        {
            FileName = $"file_{file.Id}_log.txt",
            Content = sb.ToString(),
        };
    }

    // ── POST /api/v1/files/{id}/retry ────────────────────────────────────────
    public async Task<RetryFileResponseDto?> RetryFileAsync(
        Guid fileId, CancellationToken ct = default)
    {
        // Global query filter auto-scopes to current tenant + site.
        // Include Transaction so we can update its counters in the same SaveChanges call.
        var file = await _db.Files
            .Include(f => f.Transaction)
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null)
            return null;   // 404 — also hides existence of other-tenant files

        if (file.Status != "Failed")
            throw new InvalidOperationException(
                "Only files in 'Failed' state can be retried.");

        var now = DateTime.UtcNow;
        var oldStatus = file.Status;

        // 1. Reset the file to Queued (FileStepHistory rows are intentionally kept)
        file.Status = "Queued";
        file.CurrentStep = "Queued";
        file.LastUpdatedAt = now;

        // 2. Adjust Transaction counters and recompute batch state
        var txn = file.Transaction
                  ?? throw new InvalidOperationException("Parent transaction not found.");

        txn.FailedCount = Math.Max(0, txn.FailedCount - 1);
        txn.ProcessingCount++;
        RecomputeTransactionState(txn, now);

        // 3. Write audit trail entry
        _db.Add(new ActivityLogEntry
        {
            Id = Guid.NewGuid(),
            TenantId = file.TenantId,
            SiteId = file.SiteId,
            EventType = "FILE_RETRY",
            EntityType = "File",
            EntityId = file.Id,
            EntityName = file.FileName,
            OldState = oldStatus,
            NewState = "Queued",
            TriggeredBy = _currentUser.UserId.ToString(),
            CreatedAt = now,
        });

        await _db.SaveChangesAsync(ct);

        // 4. Push the file back into the extraction pipeline
        await _queue.EnqueueAsync(fileId, ct);

        return new RetryFileResponseDto
        {
            FileId = fileId,
            NewStatus = "Queued",
            TransactionId = txn.Id,
            TransactionState = txn.State,
        };
    }

    // ── helpers ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Mirrors the recompute logic from ExtractionWorker — kept here so the
    /// service layer owns the rule in one place.
    /// </summary>
    private static void RecomputeTransactionState(Transaction t, DateTime at)
    {
        var settled = t.CompletedCount + t.FailedCount;
        var allDone = settled >= t.TotalFiles;
        t.State = allDone ? (t.FailedCount > 0 ? "Failed" : "Completed") : "Processing";
        t.LastUpdatedAt = at;
        t.CompletedAt = allDone ? at : null;
    }
}
