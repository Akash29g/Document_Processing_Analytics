namespace DocAnalytics.Service.Uploads;

// Uploads contracts

/// <summary>Request to open a new upload batch.</summary>
public sealed record CreateBatchRequest
{
    /// <summary>The number of files that will be uploaded into the batch.</summary>
    public int FileCount { get; init; }
}

/// <summary>Response returning the newly created batch id.</summary>
public sealed record CreateBatchResponse
{
    /// <summary>The created batch (transaction) id.</summary>
    public Guid BatchId { get; init; }
}

/// <summary>Request for a presigned upload URL for one file within a batch.</summary>
public sealed record UploadUrlRequest
{
    /// <summary>The target batch id.</summary>
    public Guid BatchId { get; init; }
    /// <summary>The file name (must end in .pdf).</summary>
    public string FileName { get; init; } = "";
    /// <summary>The file size in bytes (must be &gt; 0 and within the 15 MB cap).</summary>
    public long SizeBytes { get; init; }

    /// <summary>Duplicate-handling strategy: null | "replace" | "rename".</summary>
    public string? OnDuplicate { get; init; }   // null | "replace" | "rename"
}

/// <summary>Response returning the file id and its presigned PUT URL.</summary>
public sealed record UploadUrlResponse
{
    /// <summary>The created file id.</summary>
    public Guid FileId { get; init; }
    /// <summary>The presigned S3 PUT URL the client uploads to.</summary>
    public string UploadUrl { get; init; } = "";
}
