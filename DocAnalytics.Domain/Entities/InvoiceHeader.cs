namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

public class InvoiceHeader : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceDate { get; set; }   // keep string, or DateTime? if you parse it
    public string? Seller { get; set; }
    public string? Buyer { get; set; }
    public string? Currency { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Tax { get; set; }
    public decimal? Shipping { get; set; }
    public decimal? Total { get; set; }
    public DateTime ExtractedAt { get; set; }
    public FileRecord File { get; set; } = null!;
}
