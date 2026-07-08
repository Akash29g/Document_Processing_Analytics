// Entities/FileRecord.cs  -> table "files"
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
using System.Xml.Linq;

public class FileRecord : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid TransactionId { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string CurrentStep { get; set; } = null!;
    public long? FileSizeBytes { get; set; }
    public string? ExtractionStatus { get; set; }
    public decimal? ExtractionConfidence { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
    public ICollection<FileStepHistory> Steps { get; set; } = new List<FileStepHistory>();
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

    public string? StorageKey { get; set; }   // S3 object key; null for seed rows
}
