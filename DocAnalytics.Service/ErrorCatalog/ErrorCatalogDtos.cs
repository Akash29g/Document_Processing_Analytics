namespace DocAnalytics.Service.ErrorCatalog;

/// <summary>Read model for a single error catalog entry.</summary>
public sealed class ErrorCatalogDto
{
    public Guid Id { get; init; }
    public string ErrorCode { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? RemediationMsg { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

/// <summary>Body for POST /api/v1/error-catalog — create a new entry.</summary>
public sealed class CreateErrorCatalogDto
{
    public string ErrorCode { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? RemediationMsg { get; init; }
}

/// <summary>Body for PUT /api/v1/error-catalog/{id} — update description + remediation only.</summary>
public sealed class UpdateErrorCatalogDto
{
    public string Description { get; init; } = null!;
    public string? RemediationMsg { get; init; }
}
