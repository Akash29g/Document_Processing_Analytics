using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.ActivityLog;

public interface IActivityLogService
{
    Task<PagedResult<ActivityLogItemDto>> GetActivityLogAsync(
        ActivityLogQuery query, CancellationToken ct = default);
}
