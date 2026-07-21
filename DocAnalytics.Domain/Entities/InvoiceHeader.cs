namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

/// <summary>Extracted invoice header fields (1:1 with a file); tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
public class InvoiceHeader : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The file this header was extracted from (1:1).</summary>
    public Guid FileId { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }
    /// <summary>Invoice number, if present.</summary>
    public string? InvoiceNumber { get; set; }
    /// <summary>Invoice date as extracted (string; parse to DateTime? if needed).</summary>
    public string? InvoiceDate { get; set; }   // keep string, or DateTime? if you parse it
    /// <summary>The business issuing the invoice.</summary>
    public string? Seller { get; set; }
    /// <summary>The recipient being billed.</summary>
    public string? Buyer { get; set; }
    /// <summary>ISO currency code.</summary>
    public string? Currency { get; set; }
    /// <summary>Sum of line amounts before discount/tax/shipping.</summary>
    public decimal? Subtotal { get; set; }
    /// <summary>Discount amount.</summary>
    public decimal? Discount { get; set; }
    /// <summary>Tax amount.</summary>
    public decimal? Tax { get; set; }
    /// <summary>Shipping/freight amount.</summary>
    public decimal? Shipping { get; set; }
    /// <summary>Final payable grand total.</summary>
    public decimal? Total { get; set; }
    /// <summary>When the header was extracted (UTC).</summary>
    public DateTime ExtractedAt { get; set; }
    /// <summary>The parent file navigation property.</summary>
    public FileRecord File { get; set; } = null!;
}
