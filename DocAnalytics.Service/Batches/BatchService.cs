using DocAnalytics.Data;                 // ① AppDbContext lives here
using DocAnalytics.Domain.Entities;      // ① the Transaction entity
using DocAnalytics.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Batches;


/// <summary>Default <see cref="IBatchService"/> implementation backed by EF Core (tenant/site auto-scoped).</summary>
public sealed class BatchService : IBatchService
{
    private readonly AppDbContext _db;

    // ② constructor injection — the DbContext is handed to us
    public BatchService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default)
    {
        // --- normalise paging (never trust raw input) ---
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // ③ start a query. tenant_id + site_id filter is AUTO-applied.
        IQueryable<Transaction> q = _db.Transactions.AsNoTracking();

        // ④ ---- FILTERS (added only if provided) ----
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !query.Status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var state = MapStatusToState(query.Status);
            if (state is not null)
                q = q.Where(b => b.State == state);
        }

        if (!string.IsNullOrWhiteSpace(query.Source))
            q = q.Where(b => b.SourceSystem == query.Source);

        if (query.From.HasValue)
        {
            var fromUtc = query.From.Value.AsUtc();
            q = q.Where(b => b.SubmittedAt >= fromUtc);
        }
        if (query.To.HasValue)
        {
            var toUtc = query.To.Value.AsUtc();
            q = q.Where(b => b.SubmittedAt <= toUtc);
        }


        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(b => EF.Functions.ILike(b.Id.ToString(), $"%{term}%"));
        }

        // ⑤ ---- COUNT before paging ----
        var totalCount = await q.CountAsync(ct);

        // ⑥ ---- SORT (whitelisted) ----
        q = ApplySorting(q, query.SortBy, query.SortDir);

        // ⑦ ---- PAGE + SHAPE into DTOs ----
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BatchListItemDto
            {
                TransactionId = b.Id,
                State = b.State,
                SourceSystem = b.SourceSystem,
                TotalFiles = b.TotalFiles,
                UploadedCount = b.UploadedCount,
                ProcessingCount = b.ProcessingCount,
                FailedCount = b.FailedCount,
                CompletedCount = b.CompletedCount,
                SubmittedAt = b.SubmittedAt,
                LastUpdatedAt = b.LastUpdatedAt,
                CompletedAt = b.CompletedAt
            })
            .ToListAsync(ct);

        return new PagedResult<BatchListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <inheritdoc />
    // Distinct source systems for the current tenant/site (Transactions is ITenantScoped
    // → tenant_id + site_id auto-applied by the global query filter).
    public async Task<List<string>> GetSourcesAsync(CancellationToken ct = default)
    {
        return await _db.Transactions
            .AsNoTracking()
            .Select(t => t.SourceSystem)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);
    }


    /// <inheritdoc />
    // ── GET /api/v1/batches/{id} : drill into ONE batch ──
    public async Task<BatchDetailDto?> GetBatchByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        return await _db.Transactions
            .AsNoTracking()
            .Where(b => b.Id == id)            // tenant_id + site_id auto-added by the filter
            .Select(b => new BatchDetailDto
            {
                Id = b.Id,
                Status = b.State,
                Source = b.SourceSystem,
                TotalFiles = b.TotalFiles,
                FileStats = new FileStatsDto
                {
                    Uploaded = b.UploadedCount,
                    Processing = b.ProcessingCount,
                    Failed = b.FailedCount,
                    Completed = b.CompletedCount
                },
                Times = new BatchTimesDto
                {
                    SubmittedAt = b.SubmittedAt,
                    LastUpdatedAt = b.LastUpdatedAt,
                    CompletedAt = b.CompletedAt
                }
            })
            .FirstOrDefaultAsync(ct);          // null = not found
    }

    /// <inheritdoc />
    // ── GET /api/v1/batches/{id}/files : list the files in ONE batch (paged) ──
    public async Task<PagedResult<BatchFileDto>?> GetBatchFilesAsync(
        Guid id, BatchFilesQuery query, CancellationToken ct = default)
    {
        // 1. batch exists? (for this tenant/site) — if not → null → 404
        var batchExists = await _db.Transactions
            .AsNoTracking()
            .AnyAsync(b => b.Id == id, ct);

        if (!batchExists)
            return null;

        // 2. normalise paging
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // 3. ONE query against files, filtered to this batch (no N+1)
        var q = _db.Files
            .AsNoTracking()
            .Where(f => f.TransactionId == id);

        // 4. count before paging
        var totalCount = await q.CountAsync(ct);

        // 5. order → page → shape
        var items = await q
        .OrderByDescending(f => f.CreatedAt)
        .ThenBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new BatchFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                FileType = f.FileType,
                Status = f.Status,
                CurrentStep = f.CurrentStep,
                FileSizeBytes = f.FileSizeBytes,
                CreatedAt = f.CreatedAt,
                LastUpdatedAt = f.LastUpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<BatchFileDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }


    // friendly API word -> the DB's state value
    private static string? MapStatusToState(string status) =>
        status.ToLowerInvariant() switch
        {
            "failed" => "Failed",
            "completed" => "Completed",
            "in_progress" => "Processing",
            "queued" => "Queued",     // ← ADD THIS
            _ => null
        };

    // ⑥ ONLY these columns can be sorted -> blocks SQL injection via sortBy
    private static IQueryable<Transaction> ApplySorting(
        IQueryable<Transaction> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? "last_updated").ToLowerInvariant() switch
        {
            "submitted_at" => desc ? q.OrderByDescending(b => b.SubmittedAt)
                                                : q.OrderBy(b => b.SubmittedAt),
            "state" or "status" => desc ? q.OrderByDescending(b => b.State)
                                                : q.OrderBy(b => b.State),
            "source" or "source_system" => desc ? q.OrderByDescending(b => b.SourceSystem)
                                                : q.OrderBy(b => b.SourceSystem),
            "total_files" => desc ? q.OrderByDescending(b => b.TotalFiles)
                                                : q.OrderBy(b => b.TotalFiles),
            _ => desc ? q.OrderByDescending(b => b.LastUpdatedAt)
                                                : q.OrderBy(b => b.LastUpdatedAt)
        };
    }
}
