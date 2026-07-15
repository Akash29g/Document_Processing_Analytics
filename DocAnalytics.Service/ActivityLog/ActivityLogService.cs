using DocAnalytics.Data;                         // AppDbContext
using DocAnalytics.Service.Common;               // PagedResult<T>
using Microsoft.EntityFrameworkCore;
using DomainActivityLog = DocAnalytics.Domain.Entities.ActivityLog;

namespace DocAnalytics.Service.ActivityLog;

public sealed class ActivityLogService : IActivityLogService
{
    private readonly AppDbContext _db;
    public ActivityLogService(AppDbContext db) => _db = db;

    public async Task<PagedResult<ActivityLogItemDto>> GetActivityLogAsync(
        ActivityLogQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // ActivityLog IS ITenantScoped → tenant_id + site_id auto-applied by the global filter.
        IQueryable<DomainActivityLog> q = _db.ActivityLog.AsNoTracking();

        // ---- FILTERS (added only if provided) ----
        if (!string.IsNullOrWhiteSpace(query.EventType))
            q = q.Where(a => a.EventType == query.EventType);

        if (!string.IsNullOrWhiteSpace(query.EntityType))
            q = q.Where(a => a.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.Entity))
        {
            var term = query.Entity.Trim();
            q = q.Where(a => a.EntityName != null && EF.Functions.ILike(a.EntityName, $"%{term}%"));
        }

        // Postgres timestamptz requires Kind=Utc; query-string dates arrive as Kind=Unspecified.
        // (Swap for query.From.Value.AsUtc() once feature/validation's Service/Common helper merges.)
        if (query.From.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);
            q = q.Where(a => a.CreatedAt >= fromUtc);
        }

        if (query.To.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);
            q = q.Where(a => a.CreatedAt <= toUtc);
        }


        // ---- COUNT before paging ----
        var totalCount = await q.CountAsync(ct);

        // ---- SORT (whitelisted) → PAGE → SHAPE ----
        var items = await ApplySorting(q, query.SortBy, query.SortDir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ActivityLogItemDto
            {
                Ts = a.CreatedAt,
                EventType = a.EventType,
                EntityType = a.EntityType,
                Entity = a.EntityName,
                OldState = a.OldState,
                NewState = a.NewState,
                Actor = a.TriggeredBy
            })
            .ToListAsync(ct);

        return new PagedResult<ActivityLogItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<DomainActivityLog> ApplySorting(
        IQueryable<DomainActivityLog> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        IOrderedQueryable<DomainActivityLog> ordered = (sortBy ?? "ts").ToLowerInvariant() switch
        {
            "event_type" => desc ? q.OrderByDescending(a => a.EventType) : q.OrderBy(a => a.EventType),
            "entity" => desc ? q.OrderByDescending(a => a.EntityName) : q.OrderBy(a => a.EntityName),
            _ => desc ? q.OrderByDescending(a => a.CreatedAt) : q.OrderBy(a => a.CreatedAt)
        };

        return ordered.ThenBy(a => a.Id);  // stable page boundaries on tied timestamps
    }
}
