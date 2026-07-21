// Entities/DocumentType.cs  (global catalog)
namespace DocAnalytics.Domain.Entities;

/// <summary>Global catalog of supported document types (not tenant-scoped).</summary>
public class DocumentType
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The type name (e.g. Invoice).</summary>
    public string TypeName { get; set; } = null!;
    /// <summary>The file category: PDF | CSV.</summary>
    public string Category { get; set; } = null!;   // PDF | CSV
    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
    /// <summary>Whether the type is active/selectable.</summary>
    public bool IsActive { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}
