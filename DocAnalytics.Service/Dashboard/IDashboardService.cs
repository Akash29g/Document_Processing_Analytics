using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken ct = default);
    Task<PagedResult<RecentFailureDto>> GetRecentFailuresAsync(
        RecentFailuresQuery query, CancellationToken ct = default);
}
