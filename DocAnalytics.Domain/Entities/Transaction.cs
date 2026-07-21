// Entities/Transaction.cs  (the "TId" / batch)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

/// <summary>An upload batch ("TId") that groups files and tracks per-status counters; tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
public class Transaction : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }
    /// <summary>Batch state: Processing | Completed | Failed.</summary>
    public string State { get; set; } = null!;
    /// <summary>Origin of the batch (e.g. Manual_Upload).</summary>
    public string SourceSystem { get; set; } = null!;
    /// <summary>Total number of files expected in the batch.</summary>
    public int TotalFiles { get; set; }
    /// <summary>Count of files uploaded so far.</summary>
    public int UploadedCount { get; set; }
    /// <summary>Count of files currently processing.</summary>
    public int ProcessingCount { get; set; }
    /// <summary>Count of failed files.</summary>
    public int FailedCount { get; set; }
    /// <summary>Count of successfully completed files.</summary>
    public int CompletedCount { get; set; }
    /// <summary>When the batch was submitted (UTC).</summary>
    public DateTime SubmittedAt { get; set; }
    /// <summary>Timestamp of the last update (UTC).</summary>
    public DateTime LastUpdatedAt { get; set; }
    /// <summary>When the batch finished (UTC); null while in progress.</summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>The files belonging to this batch.</summary>
    public ICollection<FileRecord> Files { get; set; } = new List<FileRecord>();
}
