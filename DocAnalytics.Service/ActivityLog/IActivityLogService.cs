using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.ActivityLog;

/// <summary>Reads the append-only system audit trail (FR-4).</summary>
public interface IActivityLogService
{
    /// <summary>Returns a filtered, paginated slice of the activity log for the current tenant/site.</summary>
    /// <param name="query">Filter (event type/entity/date range) and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of activity-log items with total counts.</returns>
    Task<PagedResult<ActivityLogItemDto>> GetActivityLogAsync(
        ActivityLogQuery query, CancellationToken ct = default);
}
