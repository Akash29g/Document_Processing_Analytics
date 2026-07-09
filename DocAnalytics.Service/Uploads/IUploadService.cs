namespace DocAnalytics.Service.Uploads;

public interface IUploadService
{
    Task<UploadUrlResponse> CreateUploadAsync(UploadUrlRequest req, CancellationToken ct = default);
    Task<bool> CompleteAsync(Guid fileId, CancellationToken ct = default);

    Task<CreateBatchResponse> CreateBatchAsync(CreateBatchRequest req, CancellationToken ct = default);
    
}
