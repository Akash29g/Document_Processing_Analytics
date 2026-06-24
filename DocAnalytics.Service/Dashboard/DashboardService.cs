using DocAnalytics.Data;
using DocAnalytics.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db) => _db = db;

    // FR-1.1 — SUM the per-batch counters. tenant_id + site_id auto-applied
    // by the global query filter on Transaction (ITenantScoped).
    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken ct = default)
    {
        var summary = await _db.Transactions
            .AsNoTracking()
            .GroupBy(_ => 1)                       // collapse all rows into ONE aggregate row
            .Select(g => new DashboardSummaryResponse
            {
                Queued = g.Sum(t => t.UploadedCount),
                InProgress = g.Sum(t => t.ProcessingCount),
                Completed = g.Sum(t => t.CompletedCount),
                Failed = g.Sum(t => t.FailedCount)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardSummaryResponse(); // no rows → all zeros

        summary.Total = summary.Queued + summary.InProgress + summary.Completed + summary.Failed;
        return summary;
    }

    // FR-1.4 — start FROM Files (tenant+site auto-filtered) then join the
    // failed steps. FileStepHistory is NOT ITenantScoped, so driving from
    // Files is what keeps tenant isolation intact.
    public async Task<PagedResult<RecentFailureDto>> GetRecentFailuresAsync(
        RecentFailuresQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var baseQuery =
            from f in _db.Files.AsNoTracking()
            join s in _db.FileStepHistory.AsNoTracking() on f.Id equals s.FileId
            where s.Status == "Failed"             // matches DbSeeder literal exactly
            select new RecentFailureDto
            {
                FileId = f.Id,
                FileName = f.FileName,
                FailedStep = s.StepName,
                ErrorCode = s.ErrorCode,
                ErrorMessage = s.ErrorMessage,
                FailedAt = s.CompletedAt ?? s.StartedAt
            };

        var totalCount = await baseQuery.CountAsync(ct);

        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDir);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<RecentFailureDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<RecentFailureDto> ApplySorting(
        IQueryable<RecentFailureDto> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? "failed_at").ToLowerInvariant() switch
        {
            "file_name" => desc ? q.OrderByDescending(r => r.FileName)
                                          : q.OrderBy(r => r.FileName),
            "failed_step" or "step" => desc ? q.OrderByDescending(r => r.FailedStep)
                                          : q.OrderBy(r => r.FailedStep),
            _ => desc ? q.OrderByDescending(r => r.FailedAt)
                                          : q.OrderBy(r => r.FailedAt)
        };
    }
}
