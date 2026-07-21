// Entities/InvoiceLineItem.cs  (NEW - table 11)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

/// <summary>A single extracted invoice line item; tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
public class InvoiceLineItem : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The file this line item was extracted from.</summary>
    public Guid FileId { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }
    /// <summary>Optional link to the global item-category catalog.</summary>
    public Guid? ItemCategoryId { get; set; }
    /// <summary>1-based line number on the invoice.</summary>
    public int LineNumber { get; set; }
    /// <summary>Item description.</summary>
    public string Description { get; set; } = null!;
    /// <summary>Quantity — DECIMAL(12,3).</summary>
    public decimal? Quantity { get; set; }          // DECIMAL(12,3)
    /// <summary>Unit price — DECIMAL(12,2).</summary>
    public decimal? UnitPrice { get; set; }          // DECIMAL(12,2)
    /// <summary>Line total — DECIMAL(12,2).</summary>
    public decimal? LineTotal { get; set; }          // DECIMAL(12,2)
    /// <summary>Extraction confidence — DECIMAL(4,3).</summary>
    public decimal? Confidence { get; set; }         // DECIMAL(4,3)
    /// <summary>Whether the line passed validation.</summary>
    public bool IsValid { get; set; }
    /// <summary>When the line was extracted (UTC).</summary>
    public DateTime ExtractedAt { get; set; }
    /// <summary>The parent file navigation property.</summary>
    public FileRecord File { get; set; } = null!;
    /// <summary>The item-category navigation property, if categorized.</summary>
    public ItemCategory? ItemCategory { get; set; }
}
