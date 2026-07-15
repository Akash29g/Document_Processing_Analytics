namespace DocAnalytics.Service.Dashboard;

// FR-1.1 — status counters (snake_case'd globally → queued, in_progress, ...)
public sealed class DashboardSummaryResponse
{
    public int Queued { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Failed { get; set; }
    public int Total { get; set; }
}

// FR-1.4 — one row per failed step
public sealed class RecentFailureDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = default!;
    public string FailedStep { get; set; } = default!;
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? FailedAt { get; set; }
}

// query-string params (same naming style as BatchListQuery)
public sealed class RecentFailuresQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }   // failed_at | file_name | failed_step
    public string? SortDir { get; set; }  // asc | desc
}
