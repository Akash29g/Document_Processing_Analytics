using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Uploads;

// Uploads contracts
public sealed record CreateBatchRequest { public int FileCount { get; init; } }
public sealed record CreateBatchResponse { public Guid BatchId { get; init; } }

public sealed record UploadUrlRequest
{
    public Guid BatchId { get; init; }     
    public string FileName { get; init; } = "";
    public long SizeBytes { get; init; }
}

public sealed record UploadUrlResponse
{
    public Guid FileId { get; init; }
    public string UploadUrl { get; init; } = "";
}

