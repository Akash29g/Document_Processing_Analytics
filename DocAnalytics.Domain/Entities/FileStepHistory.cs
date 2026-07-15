// Entities/FileStepHistory.cs
namespace DocAnalytics.Domain.Entities;

public class FileStepHistory
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string StepName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public FileRecord File { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
}
