namespace DocAnalytics.Service.Uploads;

public interface IUploadService
{
    Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default);
    Task<bool> CompleteAsync(Guid fileId, CancellationToken ct = default);

    Task<CreateBatchResponse> CreateBatchAsync(CreateBatchRequest req, CancellationToken ct = default);
    Task<bool> ShrinkBatchAsync(Guid batchId, CancellationToken ct = default);

    Task<bool> DeleteBatchAsync(Guid batchId, CancellationToken ct = default);

    Task<string?> GetDownloadUrlAsync(Guid fileId, CancellationToken ct = default);




}
