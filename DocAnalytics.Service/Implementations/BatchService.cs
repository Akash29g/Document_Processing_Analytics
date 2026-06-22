using DocAnalytics.Data;                 // ① AppDbContext lives here
using DocAnalytics.Domain.Entities;      // ① the Transaction entity
using DocAnalytics.Service.Abstractions;
using DocAnalytics.Service.Common;
using DocAnalytics.Service.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Implementations;

public sealed class BatchService : IBatchService
{
    private readonly AppDbContext _db;

    // ② constructor injection — the DbContext is handed to us
    public BatchService(AppDbContext db) => _db = db;

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
            q = q.Where(b => b.SubmittedAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(b => b.SubmittedAt <= query.To.Value);

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

    // friendly API word -> the DB's state value
    private static string? MapStatusToState(string status) =>
        status.ToLowerInvariant() switch
        {
            "failed" => "Failed",
            "completed" => "Completed",
            "in_progress" => "Processing",
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
