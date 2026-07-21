// Entities/FileRecord.cs  -> table "files"
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

/// <summary>A single uploaded document (maps to the "files" table); tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
public class FileRecord : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }
    /// <summary>The parent batch (transaction) this file belongs to.</summary>
    public Guid TransactionId { get; set; }
    /// <summary>Optional classified document type.</summary>
    public Guid? DocumentTypeId { get; set; }
    /// <summary>Original file name (may be auto-renamed on same-day duplicates).</summary>
    public string FileName { get; set; } = null!;
    /// <summary>File type label (e.g. PDF).</summary>
    public string FileType { get; set; } = null!;
    /// <summary>Current processing status: Queued | Processing | Completed | Failed.</summary>
    public string Status { get; set; } = null!;
    /// <summary>Current pipeline step (e.g. Upload, Validate, Publish).</summary>
    public string CurrentStep { get; set; } = null!;
    /// <summary>File size in bytes, if known.</summary>
    public long? FileSizeBytes { get; set; }
    /// <summary>Extraction lifecycle state (e.g. Pending, Done), if applicable.</summary>
    public string? ExtractionStatus { get; set; }
    /// <summary>Overall extraction confidence score, if computed.</summary>
    public decimal? ExtractionConfidence { get; set; }
    /// <summary>Timestamp of the last state change (UTC).</summary>
    public DateTime LastUpdatedAt { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>The parent batch navigation property.</summary>
    public Transaction Transaction { get; set; } = null!;
    /// <summary>The classified document type navigation property, if any.</summary>
    public DocumentType? DocumentType { get; set; }
    /// <summary>Ordered processing-step history for this file.</summary>
    public ICollection<FileStepHistory> Steps { get; set; } = new List<FileStepHistory>();
    /// <summary>Extracted invoice line items for this file.</summary>
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

    /// <summary>S3 object key; null for seed rows.</summary>
    public string? StorageKey { get; set; }   // S3 object key; null for seed rows
}
