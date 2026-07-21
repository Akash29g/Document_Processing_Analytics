namespace DocAnalytics.Service.Errors;

/// <summary>Query-string filters/paging/sort for the error list and export (FR-3.4/3.5).</summary>
// query-string params (same naming style as BatchListQuery / RecentFailuresQuery)
public sealed class ErrorListQuery
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;
    /// <summary>Page size (capped at 100 by the service).</summary>
    public int PageSize { get; set; } = 20;
    /// <summary>Include failures on/after this instant (ISO-8601).</summary>
    public DateTime? From { get; set; }      // failed on/after  (ISO-8601)
    /// <summary>Include failures on/before this instant (ISO-8601).</summary>
    public DateTime? To { get; set; }        // failed on/before (ISO-8601)
    /// <summary>Processing step filter: Upload | Validate | Transform | Load.</summary>
    public string? Step { get; set; }        // Upload | Validate | Transform | Load
    /// <summary>Source-system filter (Transaction.SourceSystem).</summary>
    public string? Source { get; set; }      // source system (Transaction.SourceSystem)
    /// <summary>Sort column: failed_at | file_name | error_code | step | source.</summary>
    public string? SortBy { get; set; }      // failed_at | file_name | error_code | step | source
    /// <summary>Sort direction: asc | desc.</summary>
    public string? SortDir { get; set; }     // asc | desc
}

/// <summary>One failed-step row returned to the client / exported to CSV.</summary>
// one failed-step row
public sealed class ErrorListItemDto
{
    /// <summary>The file id.</summary>
    public Guid FileId { get; set; }
    /// <summary>The file name.</summary>
    public string FileName { get; set; } = null!;
    /// <summary>The error code recorded for the failed step.</summary>
    public string ErrorCode { get; set; } = null!;
    /// <summary>The error message, if any.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>The step that failed.</summary>
    public string Step { get; set; } = null!;        // failed step name
    /// <summary>The source system.</summary>
    public string Source { get; set; } = null!;      // source system
    /// <summary>When the failure occurred (UTC).</summary>
    public DateTime? FailedAt { get; set; }
    /// <summary>Suggested remediation from ErrorCatalog (LEFT join), if available.</summary>
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (LEFT join)
}
