// Entities/Transaction.cs  (the "TId" / batch)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class Transaction : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string State { get; set; } = null!;
    public string SourceSystem { get; set; } = null!;
    public int TotalFiles { get; set; }
    public int UploadedCount { get; set; }
    public int ProcessingCount { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ICollection<FileRecord> Files { get; set; } = new List<FileRecord>();
}
