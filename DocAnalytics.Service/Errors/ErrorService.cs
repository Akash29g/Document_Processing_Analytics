using DocAnalytics.Data;                 // AppDbContext
using DocAnalytics.Service.Common;       // PagedResult<T>
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Errors;

/// <summary>Default <see cref="IErrorService"/> implementation: filtered failed-step queries and CSV export rows.</summary>
public sealed class ErrorService : IErrorService
{
    private readonly AppDbContext _db;
    public ErrorService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(
        ErrorListQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var baseQuery = BuildFilteredQuery(query);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await ApplySorting(baseQuery, query.SortBy, query.SortDir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // ── NEW: resolve BatchId so the frontend can deep-link to the file ──
        var fileIds = items.Select(i => i.FileId).Distinct().ToList();

        if (fileIds.Count > 0 && _db.Files is not null)
        {
            var batchMap = await _db.Files
                .Where(f => fileIds.Contains(f.Id))
                .Select(f => new { f.Id, f.TransactionId })
                .ToDictionaryAsync(f => f.Id, f => f.TransactionId, ct);

            for (var i = 0; i < items.Count; i++)
            {
                if (!batchMap.TryGetValue(items[i].FileId, out var txnId)) continue;
                items[i] = items[i] with { BatchId = txnId };
            }
        }
        // ─────────────────────────────────────────────────────────────────────


        return new PagedResult<ErrorListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <inheritdoc />
    public async Task<List<ErrorListItemDto>> GetErrorsForExportAsync(
        ErrorListQuery query, CancellationToken ct = default)
        => await ApplySorting(BuildFilteredQuery(query), query.SortBy, query.SortDir)
            .ToListAsync(ct);

    // ── core query: Files (scoped) → FileStepHistory → Transactions (scoped) → ErrorCatalog (LEFT) ──
    private IQueryable<ErrorListItemDto> BuildFilteredQuery(ErrorListQuery query)
    {
        // tenant_id + site_id auto-applied to Files AND Transactions by the global filter.
        var q =
            from f in _db.Files.AsNoTracking()
            join s in _db.FileStepHistory.AsNoTracking() on f.Id equals s.FileId
            join t in _db.Transactions.AsNoTracking() on f.TransactionId equals t.Id
            join ec in _db.ErrorCatalog.AsNoTracking() on s.ErrorCode equals ec.ErrorCode into ecg
            from ec in ecg.DefaultIfEmpty()        // LEFT join → suggested_fix null if no catalog row
            where s.Status == "Failed"             // matches DbSeeder literal exactly
            select new { f, s, t, ec };

        // Postgres timestamptz requires Kind=Utc; query-string dates arrive as Kind=Unspecified.
        if (query.From.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);
            q = q.Where(x => (x.s.CompletedAt ?? x.s.StartedAt) >= fromUtc);
        }

        if (query.To.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);
            q = q.Where(x => (x.s.CompletedAt ?? x.s.StartedAt) <= toUtc);
        }

        if (!string.IsNullOrWhiteSpace(query.Step))
            q = q.Where(x => x.s.StepName == query.Step);

        if (!string.IsNullOrWhiteSpace(query.Source))
            q = q.Where(x => x.t.SourceSystem == query.Source);

        return q.Select(x => new ErrorListItemDto
        {
            FileId = x.f.Id,
            FileName = x.f.FileName,
            ErrorCode = x.s.ErrorCode!,            // failed steps always carry a code in seed
            ErrorMessage = x.s.ErrorMessage,
            Step = x.s.StepName,
            Source = x.t.SourceSystem,
            FailedAt = x.s.CompletedAt ?? x.s.StartedAt,
            SuggestedFix = x.ec != null ? x.ec.RemediationMsg : null
        });
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<ErrorListItemDto> ApplySorting(
        IQueryable<ErrorListItemDto> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        IOrderedQueryable<ErrorListItemDto> ordered = (sortBy ?? "failed_at").ToLowerInvariant() switch
        {
            "file_name" => desc ? q.OrderByDescending(r => r.FileName) : q.OrderBy(r => r.FileName),
            "error_code" or "code" => desc ? q.OrderByDescending(r => r.ErrorCode) : q.OrderBy(r => r.ErrorCode),
            "step" => desc ? q.OrderByDescending(r => r.Step) : q.OrderBy(r => r.Step),
            "source" => desc ? q.OrderByDescending(r => r.Source) : q.OrderBy(r => r.Source),
            _ => desc ? q.OrderByDescending(r => r.FailedAt) : q.OrderBy(r => r.FailedAt)
        };

        return ordered.ThenBy(r => r.FileId);  // stable page boundaries on tied timestamps
    }
}
