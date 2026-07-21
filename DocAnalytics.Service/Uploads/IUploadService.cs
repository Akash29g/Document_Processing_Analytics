namespace DocAnalytics.Service.Uploads;

/// <summary>Handles file uploads and batch lifecycle: presigned URLs, completion, and batch create/shrink/delete.</summary>
public interface IUploadService
{
    /// <summary>Creates a file record and returns a presigned URL for uploading its bytes.</summary>
    /// <param name="req">File name, size, and duplicate-handling choice.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The presigned upload URL and the new file id.</returns>
    Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default);

    /// <summary>Marks an uploaded file complete and enqueues it for extraction.</summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if completed; <c>false</c> if the file was not found.</returns>
    Task<bool> CompleteAsync(Guid fileId, CancellationToken ct = default);

    /// <summary>Creates a new upload batch (transaction) for a planned set of files.</summary>
    /// <param name="req">The batch definition (e.g. file count).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created batch.</returns>
    Task<CreateBatchResponse> CreateBatchAsync(CreateBatchRequest req, CancellationToken ct = default);

    /// <summary>Decrements a batch's expected file count when a planned file is skipped; removes the batch if it empties.</summary>
    /// <param name="batchId">The batch id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if shrunk; <c>false</c> if the batch was not found.</returns>
    Task<bool> ShrinkBatchAsync(Guid batchId, CancellationToken ct = default);

    /// <summary>Deletes a batch, its files, and the associated stored objects.</summary>
    /// <param name="batchId">The batch id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if deleted; <c>false</c> if the batch was not found.</returns>
    Task<bool> DeleteBatchAsync(Guid batchId, CancellationToken ct = default);

    /// <summary>Returns a short-lived presigned S3 GET URL for a file's stored document.</summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The presigned download URL, or <c>null</c> if the file is missing or has no stored document.</returns>
    Task<string?> GetDownloadUrlAsync(Guid fileId, CancellationToken ct = default);




}
