namespace DocAnalytics.Service.Storage;

/// <summary>Abstraction over object storage (S3): key building, presigned URLs, and read/delete operations.</summary>
public interface IFileStorage
{
    /// <summary>Builds the tenant-scoped storage key for a file.</summary>
    /// <param name="tenantName">The tenant name.</param>
    /// <param name="siteName">The site name.</param>
    /// <param name="dateUtc">The upload date (UTC), used in the key path.</param>
    /// <param name="fileName">The original file name.</param>
    /// <returns>The computed storage key.</returns>
    string BuildKey(string tenantName, string siteName, DateTime dateUtc, string fileName);

    /// <summary>Builds a time-limited presigned URL for downloading a stored object.</summary>
    /// <param name="storageKey">The object's storage key.</param>
    /// <param name="fileName">The download file name to present.</param>
    /// <param name="validFor">How long the URL remains valid.</param>
    /// <returns>The presigned download URL.</returns>
    string GetDownloadUrl(string storageKey, string fileName, TimeSpan validFor);

    /// <summary>Deletes a stored object.</summary>
    /// <param name="storageKey">The object's storage key.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(string storageKey, CancellationToken ct = default);

    /// <summary>Builds a short-lived presigned URL the browser uses to PUT bytes straight to S3.</summary>
    /// <param name="key">The target storage key.</param>
    /// <param name="contentType">The content type of the upload.</param>
    /// <param name="ttl">How long the URL remains valid.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The presigned upload (PUT) URL.</returns>
    Task<string> GetPresignedPutUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct = default);

    /// <summary>Reads an object's bytes back (used by the extraction worker).</summary>
    /// <param name="key">The object's storage key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The object's raw bytes.</returns>
    Task<byte[]> DownloadAsync(string key, CancellationToken ct = default);

    /// <summary>Returns the malware-scan status for a stored object, if available.</summary>
    /// <param name="storageKey">The object's storage key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The scan status, or <c>null</c> if not available.</returns>
    Task<string?> GetMalwareScanStatusAsync(string storageKey, CancellationToken ct = default);

}
