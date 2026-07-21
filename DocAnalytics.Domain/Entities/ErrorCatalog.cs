// Entities/ErrorCatalog.cs  (global)
namespace DocAnalytics.Domain.Entities;

/// <summary>Global catalog mapping error codes to descriptions and remediation guidance (not tenant-scoped).</summary>
public class ErrorCatalog
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>The unique error code.</summary>
    public string ErrorCode { get; set; } = null!;
    /// <summary>Human-readable error description.</summary>
    public string Description { get; set; } = null!;
    /// <summary>Suggested remediation message, if any.</summary>
    public string? RemediationMsg { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Last update timestamp (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}
