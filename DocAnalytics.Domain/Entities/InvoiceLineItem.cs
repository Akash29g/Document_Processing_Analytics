// Entities/InvoiceLineItem.cs  (NEW - table 11)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class InvoiceLineItem : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid? ItemCategoryId { get; set; }
    public int LineNumber { get; set; }
    public string Description { get; set; } = null!;
    public decimal? Quantity { get; set; }          // DECIMAL(12,3)
    public decimal? UnitPrice { get; set; }          // DECIMAL(12,2)
    public decimal? LineTotal { get; set; }          // DECIMAL(12,2)
    public decimal? Confidence { get; set; }         // DECIMAL(4,3)
    public bool IsValid { get; set; }
    public DateTime ExtractedAt { get; set; }
    public FileRecord File { get; set; } = null!;
    public ItemCategory? ItemCategory { get; set; }
}
