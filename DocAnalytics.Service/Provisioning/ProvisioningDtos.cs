using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Provisioning;

// ── requests ──
public sealed record CreateTenantRequest(
    [Required, MaxLength(120)] string Name,
    [Required, MaxLength(100), RegularExpression(@"^[a-z0-9.-]+\.[a-z]{2,}$",
        ErrorMessage = "org_domain must be a valid domain, e.g. acme.com")] string OrgDomain);

public sealed record CreateAdminRequest(
    [Required, MaxLength(60)] string FirstName,
    [Required, MaxLength(60)] string LastName);

public sealed record CreateSiteRequest(
    [Required, MaxLength(120)] string Name,
    [MaxLength(120)] string? Location);

// ── responses ──
public sealed record TenantSummaryDto(
    Guid Id, string Name, string OrgDomain, bool IsActive,
    int SiteCount, int UserCount, int AdminCount);

public sealed record ProvisionedUserDto(
    Guid Id, string Email, string Role, bool IsActive, DateTime CreatedAt);

public sealed record ProvisionedSiteDto(
    Guid Id, string Name, string? Location, bool IsActive);
