using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.Errors;

/// <summary>Queries the failed-step error list and produces its CSV export (FR-3.4, FR-3.5).</summary>
public interface IErrorService
{
    /// <summary>Returns a filtered, paginated list of errors for the current tenant/site (FR-3.4).</summary>
    /// <param name="query">Filter, sort, and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of error list items with total counts.</returns>
    Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(
        ErrorListQuery query, CancellationToken ct = default);

    /// <summary>Returns ALL matching error rows (no paging) for CSV export (FR-3.5).</summary>
    /// <param name="query">The same filter/sort parameters as the list query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Every error row matching the filter.</returns>
    // export = same filters/sort, but ALL matching rows (no paging)
    Task<List<ErrorListItemDto>> GetErrorsForExportAsync(
        ErrorListQuery query, CancellationToken ct = default);
}
