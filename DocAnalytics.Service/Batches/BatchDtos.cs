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

// ── GET /api/v1/batches/{id} : the rich "drill-down" view of ONE batch ──
public class BatchDetailDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;   // mapped from Transaction.State
    public string Source { get; set; } = null!;   // mapped from Transaction.SourceSystem
    public int TotalFiles { get; set; }

    public FileStatsDto FileStats { get; set; } = null!;  // → JSON "file_stats"
    public BatchTimesDto Times { get; set; } = null!;     // → JSON "times"
}

// The 4 pre-aggregated counters, grouped into one nested object
public class FileStatsDto
{
    public int Uploaded { get; set; }
    public int Processing { get; set; }
    public int Failed { get; set; }
    public int Completed { get; set; }
}

// The 3 timestamps, grouped into one nested object
public class BatchTimesDto
{
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }   // nullable — unfinished batch has no end time
}
