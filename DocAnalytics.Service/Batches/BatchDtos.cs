namespace DocAnalytics.Service.Batches;


// The filters/options the client sends in the URL (?page=1&status=failed...)
public sealed class BatchListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }   // all | in_progress | completed | failed
    public string? Source { get; set; }   // filter by source system
    public DateTime? From { get; set; }    // submitted on/after
    public DateTime? To { get; set; }      // submitted on/before
    public string? Search { get; set; }    // partial batch id
    public string? SortBy { get; set; }    // which column to sort by
    public string? SortDir { get; set; }   // asc or desc
}

// One row of the batch list that we send back
public sealed class BatchListItemDto
{
    public Guid TransactionId { get; set; }
    public string State { get; set; } = default!;
    public string SourceSystem { get; set; } = default!;
    public int TotalFiles { get; set; }
    public int UploadedCount { get; set; }
    public int ProcessingCount { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
