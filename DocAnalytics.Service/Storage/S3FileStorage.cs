using Amazon.S3;
using Amazon.S3.Model;
using DocAnalytics.Service.Aws;
using Microsoft.Extensions.Options;

namespace DocAnalytics.Service.Storage;

public sealed class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly AwsOptions _opts;

    public S3FileStorage(IAmazonS3 s3, IOptions<AwsOptions> opts)
    {
        _s3 = s3;
        _opts = opts.Value;
    }

    public string BuildKey(Guid tenantId, Guid siteId, Guid fileId) =>
        $"tenants/{tenantId}/sites/{siteId}/files/{fileId}.pdf";

    public Task<string> GetPresignedPutUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct = default)
    {
        var req = new GetPreSignedUrlRequest
        {
            BucketName = _opts.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(ttl),
            ContentType = contentType,
        };
        return _s3.GetPreSignedURLAsync(req);
    }

    public async Task<byte[]> DownloadAsync(string key, CancellationToken ct = default)
    {
        using var resp = await _s3.GetObjectAsync(_opts.BucketName, key, ct);
        using var ms = new MemoryStream();
        await resp.ResponseStream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }
}
