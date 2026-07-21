namespace DocAnalytics.Service.Dashboard;

/// <summary>Dashboard status counters (FR-1.1); serialized to snake_case.</summary>
// FR-1.1 — status counters (snake_case'd globally → queued, in_progress, ...)
public sealed class DashboardSummaryResponse
{
    /// <summary>Files waiting to be processed.</summary>
    public int Queued { get; set; }
    /// <summary>Files currently being processed.</summary>
    public int InProgress { get; set; }
    /// <summary>Successfully completed files.</summary>
    public int Completed { get; set; }
    /// <summary>Failed files.</summary>
    public int Failed { get; set; }
    /// <summary>Sum of all status counts.</summary>
    public int Total { get; set; }
}

/// <summary>One recent failed-step row (FR-1.4).</summary>
// FR-1.4 — one row per failed step
public sealed class RecentFailureDto
{
    /// <summary>The file id.</summary>
    public Guid FileId { get; set; }
    /// <summary>The file name.</summary>
    public string FileName { get; set; } = default!;
    /// <summary>The step at which the file failed.</summary>
    public string FailedStep { get; set; } = default!;
    /// <summary>The error code, if any.</summary>
    public string? ErrorCode { get; set; }
    /// <summary>The error message, if any.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>When the failure occurred (UTC).</summary>
    public DateTime? FailedAt { get; set; }
}

/// <summary>Query-string parameters for the recent-failures table.</summary>
// query-string params (same naming style as BatchListQuery)
public sealed class RecentFailuresQuery
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;
    /// <summary>Page size (capped at 100 by the service).</summary>
    public int PageSize { get; set; } = 20;
    /// <summary>Sort column: failed_at | file_name | failed_step.</summary>
    public string? SortBy { get; set; }   // failed_at | file_name | failed_step
    /// <summary>Sort direction: asc | desc.</summary>
    public string? SortDir { get; set; }  // asc | desc
}
