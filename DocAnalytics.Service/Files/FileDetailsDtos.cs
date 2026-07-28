namespace DocAnalytics.Service.Files;

/// <summary>Nested response for GET /api/v1/files/{id}/details: file info plus its step timeline.</summary>
// ── GET /api/v1/files/{id}/details : the nested DTO ──
public sealed class FileDetailDto
{
    /// <summary>Basic file information block.</summary>
    public FileInfoDto FileInfo { get; set; } = null!;          // → "file_info"
    /// <summary>The ordered step history for the file.</summary>
    public List<StepHistoryDto> History { get; set; } = new();  // → "history"
}

/// <summary>Basic identity and current-state info for a file.</summary>
public sealed class FileInfoDto
{
    /// <summary>The file id.</summary>
    public Guid Id { get; set; }
    /// <summary>The file name.</summary>
    public string Name { get; set; } = null!;          // → "name"
    /// <summary>The current processing status.</summary>
    public string CurrentStatus { get; set; } = null!; // → "current_status"
    /// <summary>The current processing step.</summary>
    public string CurrentStep { get; set; } = null!;   // → "current_step"
}

/// <summary>One entry in a file's processing timeline.</summary>
public sealed class StepHistoryDto
{
    /// <summary>The step name (FileStepHistory.StepName).</summary>
    public string Step { get; set; } = null!;     // FileStepHistory.StepName
    /// <summary>The step outcome: Success | Failed | Processing.</summary>
    public string Status { get; set; } = null!;   // Success | Failed | Processing
    /// <summary>The step timestamp.</summary>
    public DateTime? Ts { get; set; }             // step timestamp
    /// <summary>Error details, present only on failed steps.</summary>
    public StepErrorDto? Error { get; set; }      // only present on failed steps
}

/// <summary>Error details attached to a failed step.</summary>
public sealed class StepErrorDto
{
    /// <summary>The error code (FileStepHistory.ErrorCode).</summary>
    public string Code { get; set; } = null!;        // FileStepHistory.ErrorCode
    /// <summary>The error message (FileStepHistory.ErrorMessage).</summary>
    public string? Message { get; set; }             // FileStepHistory.ErrorMessage
    /// <summary>Suggested remediation from ErrorCatalog, looked up by code.</summary>
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (by code)
}

/// <summary>Downloadable plain-text log payload for a file.</summary>
// ── downloadable logs payload ──
public sealed class FileLogDto
{
    /// <summary>The suggested download file name (e.g. file_xxx_log.txt).</summary>
    public string FileName { get; set; } = null!;   // e.g. file_xxx_log.txt
    /// <summary>The plain-text step-by-step trace.</summary>
    public string Content { get; set; } = null!;    // plain-text trace
}

// ── POST /api/v1/files/{id}/retry ──
/// <summary>Response returned after successfully re-queuing a failed file.</summary>
public sealed class RetryFileResponseDto
{
    public Guid FileId { get; init; }
    public string NewStatus { get; init; } = "Queued";
    public Guid TransactionId { get; init; }
    public string TransactionState { get; init; } = string.Empty;
}
