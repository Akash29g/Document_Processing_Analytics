// Entities/ErrorCatalog.cs  (global)
namespace DocAnalytics.Domain.Entities;

public class ErrorCatalog
{
    public Guid Id { get; set; }
    public string ErrorCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? RemediationMsg { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
