namespace DocAnalytics.Service.Files;

/// <summary>Reads a file's step-history details and builds downloadable log content (FR-2.5, FR-3.3).</summary>
public interface IFileDetailsService
{
    /// <summary>Gets a file's info plus its full step history (joins Files + FileStepHistory + ErrorCatalog).</summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The file details, or <c>null</c> if not found / not in the caller's tenant.</returns>
    Task<FileDetailDto?> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default);

    /// <summary>Builds a downloadable plain-text log from a file's step history.</summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The log content, or <c>null</c> if the file does not exist.</returns>
    Task<FileLogDto?> GetFileLogsAsync(Guid fileId, CancellationToken ct = default);

    /// <summary>
    /// Resets a Failed file back to Queued, adjusts Transaction counters, writes an
    /// audit log entry, and re-enqueues the file for processing. Admin-only operation.
    /// </summary>
    /// <returns>The updated state, or <c>null</c> if the file was not found.</returns>
    Task<RetryFileResponseDto?> RetryFileAsync(Guid fileId, CancellationToken ct = default);
}
