using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Dashboard;

/// <summary>Provides dashboard summary counters and the recent-failures table (FR-1.1, FR-1.4).</summary>
public interface IDashboardService
{
    /// <summary>Returns the aggregated status counters for the current tenant/site (FR-1.1).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Queued/in-progress/completed/failed totals.</returns>
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken ct = default);

    /// <summary>Returns a paginated, sortable list of the most recent failed files (FR-1.4).</summary>
    /// <param name="query">Pagination and sort parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of recent failures with total counts.</returns>
    Task<PagedResult<RecentFailureDto>> GetRecentFailuresAsync(
        RecentFailuresQuery query, CancellationToken ct = default);
}
