// Entities/DocumentType.cs  (global catalog)
namespace DocAnalytics.Domain.Entities;

public class DocumentType
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = null!;
    public string Category { get; set; } = null!;   // PDF | CSV
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
