using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using DocAnalytics.Service.Aws;
using Microsoft.Extensions.Options;

namespace DocAnalytics.Service.Storage;

[ExcludeFromCodeCoverage]
public sealed class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly AwsOptions _opts;

    public S3FileStorage(IAmazonS3 s3, IOptions<AwsOptions> opts)
    {
        _s3 = s3;
        _opts = opts.Value;
    }

    public string BuildKey(string tenantName, string siteName, DateTime dateUtc, string fileName)
    => $"{Slug(tenantName)}/{Slug(siteName)}/{dateUtc:yyyy/MM/dd}/{SanitizeFileName(fileName)}";

    public string GetDownloadUrl(string storageKey, string fileName, TimeSpan validFor)
    {
        var req = new GetPreSignedUrlRequest
        {
            BucketName = _opts.BucketName,
            Key = storageKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(validFor),
            // forces "Save as invoice_x.pdf" instead of opening a GUID-named tab
            ResponseHeaderOverrides = new ResponseHeaderOverrides
            {
                ContentDisposition = $"attachment; filename=\"{fileName}\""
            }
        };
        return _s3.GetPreSignedURL(req);
    }

    public async Task<string?> GetMalwareScanStatusAsync(string storageKey, CancellationToken ct = default)
    {
        var res = await _s3.GetObjectTaggingAsync(new GetObjectTaggingRequest
        {
            BucketName = _opts.BucketName,
            Key = storageKey
        }, ct);
        return res.Tagging.FirstOrDefault(t => t.Key == "GuardDutyMalwareScanStatus")?.Value;
    }


    private static string Slug(string s) =>
        Regex.Replace(s.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');

    private static string SanitizeFileName(string name) =>
        Regex.Replace(name.Trim(), @"[^\w\s\.\-\(\)']", "_");   // keep letters/digits/space/._-()' 

    public Task DeleteAsync(string storageKey, CancellationToken ct = default) =>
    _s3.DeleteObjectAsync(_opts.BucketName, storageKey, ct);


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
