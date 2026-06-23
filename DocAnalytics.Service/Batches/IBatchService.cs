using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;

public interface IBatchService
{
    Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default);
}
