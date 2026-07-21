using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;

/// <summary>Queries batches (transactions) and the files within them (FR-2).</summary>
public interface IBatchService
{
    /// <summary>Returns a filtered, paginated list of batches for the current tenant/site (FR-2.1–2.3).</summary>
    /// <param name="query">Filter, search, sort, and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of batch list items with total counts.</returns>
    Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default);

    /// <summary>Returns the detail summary for a single batch (FR-2.4).</summary>
    /// <param name="id">The batch (transaction) id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The batch detail, or <c>null</c> if not found for the current tenant/site.</returns>
    Task<BatchDetailDto?> GetBatchByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the distinct source systems used by batches, for the FilterBar dropdown.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of distinct source-system names.</returns>
    Task<List<string>> GetSourcesAsync(CancellationToken ct = default);

    /// <summary>Returns a paginated list of the files contained in a batch (FR-2.4).</summary>
    /// <param name="id">The batch (transaction) id.</param>
    /// <param name="query">Pagination and sort parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of the batch's files, or <c>null</c> if the batch is not found.</returns>
    Task<PagedResult<BatchFileDto>?> GetBatchFilesAsync(
        Guid id, BatchFilesQuery query, CancellationToken ct = default);

}
