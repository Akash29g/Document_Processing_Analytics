namespace DocAnalytics.Service.Files;

// ── GET /api/v1/files/{id}/details : the nested DTO ──
public sealed class FileDetailDto
{
    public FileInfoDto FileInfo { get; set; } = null!;          // → "file_info"
    public List<StepHistoryDto> History { get; set; } = new();  // → "history"
}

public sealed class FileInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;          // → "name"
    public string CurrentStatus { get; set; } = null!; // → "current_status"
    public string CurrentStep { get; set; } = null!;   // → "current_step"
}

public sealed class StepHistoryDto
{
    public string Step { get; set; } = null!;     // FileStepHistory.StepName
    public string Status { get; set; } = null!;   // Success | Failed | Processing
    public DateTime? Ts { get; set; }             // step timestamp
    public StepErrorDto? Error { get; set; }      // only present on failed steps
}

public sealed class StepErrorDto
{
    public string Code { get; set; } = null!;        // FileStepHistory.ErrorCode
    public string? Message { get; set; }             // FileStepHistory.ErrorMessage
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (by code)
}

// ── downloadable logs payload ──
public sealed class FileLogDto
{
    public string FileName { get; set; } = null!;   // e.g. file_xxx_log.txt
    public string Content { get; set; } = null!;    // plain-text trace
}


