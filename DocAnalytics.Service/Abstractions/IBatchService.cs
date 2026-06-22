using DocAnalytics.Service.Common;
using DocAnalytics.Service.Dtos;

namespace DocAnalytics.Service.Abstractions;

public interface IBatchService
{
    Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default);
}
