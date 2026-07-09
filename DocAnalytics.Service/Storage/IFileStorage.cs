namespace DocAnalytics.Service.Storage;

public interface IFileStorage
{
    /// Build the tenant-scoped key for a file.
    string BuildKey(string tenantName, string siteName, DateTime dateUtc, string fileName);

    string GetDownloadUrl(string storageKey, string fileName, TimeSpan validFor);

    Task DeleteAsync(string storageKey, CancellationToken ct = default);


    /// Short-lived URL the browser uses to PUT the bytes straight to S3.
    Task<string> GetPresignedPutUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct = default);

    /// Worker reads the bytes back for extraction.
    Task<byte[]> DownloadAsync(string key, CancellationToken ct = default);
}
