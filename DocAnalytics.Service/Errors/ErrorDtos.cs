namespace DocAnalytics.Service.Errors;

// query-string params (same naming style as BatchListQuery / RecentFailuresQuery)
public sealed class ErrorListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? From { get; set; }      // failed on/after  (ISO-8601)
    public DateTime? To { get; set; }        // failed on/before (ISO-8601)
    public string? Step { get; set; }        // Upload | Validate | Transform | Load
    public string? Source { get; set; }      // source system (Transaction.SourceSystem)
    public string? SortBy { get; set; }      // failed_at | file_name | error_code | step | source
    public string? SortDir { get; set; }     // asc | desc
}

// one failed-step row
public sealed class ErrorListItemDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public string Step { get; set; } = null!;        // failed step name
    public string Source { get; set; } = null!;      // source system
    public DateTime? FailedAt { get; set; }
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (LEFT join)
}
