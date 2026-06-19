// Entities/ItemCategory.cs  (NEW - table 12, master catalog, global)
namespace DocAnalytics.Domain.Entities;

public class ItemCategory
{
    public Guid Id { get; set; }
    public string CategoryCode { get; set; } = null!;   // UNIQUE
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}
