using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.Errors;

public interface IErrorService
{
    Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(
        ErrorListQuery query, CancellationToken ct = default);

    // export = same filters/sort, but ALL matching rows (no paging)
    Task<List<ErrorListItemDto>> GetErrorsForExportAsync(
        ErrorListQuery query, CancellationToken ct = default);
}
