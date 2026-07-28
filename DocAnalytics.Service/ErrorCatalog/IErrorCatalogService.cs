namespace DocAnalytics.Service.ErrorCatalog;

/// <summary>CRUD management for the global error catalog (Admin only for writes).</summary>
public interface IErrorCatalogService
{
    /// <summary>Returns every entry ordered by error code.</summary>
    Task<List<ErrorCatalogDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a new entry. Returns <c>null</c> if the error code already exists (409 Conflict).
    /// </summary>
    Task<ErrorCatalogDto?> CreateAsync(CreateErrorCatalogDto dto, CancellationToken ct = default);

    /// <summary>
    /// Updates description + remediation of an existing entry.
    /// Returns <c>null</c> if not found (404).
    /// </summary>
    Task<ErrorCatalogDto?> UpdateAsync(Guid id, UpdateErrorCatalogDto dto, CancellationToken ct = default);
}
