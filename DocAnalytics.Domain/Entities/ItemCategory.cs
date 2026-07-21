// Entities/ItemCategory.cs  (NEW - table 12, master catalog, global)
namespace DocAnalytics.Domain.Entities;

/// <summary>Global master catalog of item categories (not tenant-scoped).</summary>
public class ItemCategory
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>Unique category code.</summary>
    public string CategoryCode { get; set; } = null!;   // UNIQUE
    /// <summary>Human-readable category name.</summary>
    public string CategoryName { get; set; } = null!;
    /// <summary>Optional category description.</summary>
    public string? Description { get; set; }
    /// <summary>Whether the category is active/selectable.</summary>
    public bool IsActive { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Line items classified under this category.</summary>
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}
