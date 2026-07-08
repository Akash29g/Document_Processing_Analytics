namespace DocAnalytics.Service.Storage;

public interface IFileStorage
{
    /// Build the tenant-scoped key for a file.
    string BuildKey(Guid tenantId, Guid siteId, Guid fileId);

    /// Short-lived URL the browser uses to PUT the bytes straight to S3.
    Task<string> GetPresignedPutUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct = default);

    /// Worker reads the bytes back for extraction.
    Task<byte[]> DownloadAsync(string key, CancellationToken ct = default);
}
