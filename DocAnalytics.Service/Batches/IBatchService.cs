using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;

public interface IBatchService
{
    Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default);

    Task<BatchDetailDto?> GetBatchByIdAsync(Guid id, CancellationToken ct = default);

    Task<List<string>> GetSourcesAsync(CancellationToken ct = default);

    Task<PagedResult<BatchFileDto>?> GetBatchFilesAsync(
        Guid id, BatchFilesQuery query, CancellationToken ct = default);

}
