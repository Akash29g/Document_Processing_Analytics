// Entities/FileStepHistory.cs
namespace DocAnalytics.Domain.Entities;

/// <summary>One processing-step record in a file's timeline (NOT tenant-scoped — always reached via its parent file).</summary>
public class FileStepHistory
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The file this step belongs to.</summary>
    public Guid FileId { get; set; }
    /// <summary>Optional document type in effect for this step.</summary>
    public Guid? DocumentTypeId { get; set; }
    /// <summary>The step name (e.g. Upload, Validate, Transform, Load).</summary>
    public string StepName { get; set; } = null!;
    /// <summary>The step outcome: Success | Failed | Processing.</summary>
    public string Status { get; set; } = null!;
    /// <summary>When the step started (UTC), if recorded.</summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>When the step completed (UTC), if recorded.</summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>Error code on failure, if any.</summary>
    public string? ErrorCode { get; set; }
    /// <summary>Error message on failure, if any.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>The parent file navigation property.</summary>
    public FileRecord File { get; set; } = null!;
    /// <summary>The document type navigation property, if any.</summary>
    public DocumentType? DocumentType { get; set; }
}
