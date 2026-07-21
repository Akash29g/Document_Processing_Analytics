using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Provisioning;

// ── requests ──

/// <summary>Request to create a new tenant.</summary>
/// <param name="Name">The tenant display name.</param>
/// <param name="OrgDomain">The organization domain (e.g. acme.com).</param>
public sealed record CreateTenantRequest(
    [Required, MaxLength(120)] string Name,
    [Required, MaxLength(100), RegularExpression(@"^[a-z0-9.-]+\.[a-z]{2,}$",
        ErrorMessage = "org_domain must be a valid domain, e.g. acme.com")] string OrgDomain);

/// <summary>Request to create a tenant admin user.</summary>
/// <param name="FirstName">The admin's first name.</param>
/// <param name="LastName">The admin's last name.</param>
public sealed record CreateAdminRequest(
    [Required, MaxLength(60)] string FirstName,
    [Required, MaxLength(60)] string LastName);

/// <summary>Request to create a site under a tenant.</summary>
/// <param name="Name">The site name.</param>
/// <param name="Location">The optional site location.</param>
public sealed record CreateSiteRequest(
    [Required, MaxLength(120)] string Name,
    [MaxLength(120)] string? Location);

// ── responses ──

/// <summary>Tenant summary with active site/user/admin counts.</summary>
/// <param name="Id">The tenant id.</param>
/// <param name="Name">The tenant name.</param>
/// <param name="OrgDomain">The organization domain.</param>
/// <param name="IsActive">Whether the tenant is active.</param>
/// <param name="SiteCount">Number of active sites.</param>
/// <param name="UserCount">Number of active users.</param>
/// <param name="AdminCount">Number of active admins.</param>
public sealed record TenantSummaryDto(
    Guid Id, string Name, string OrgDomain, bool IsActive,
    int SiteCount, int UserCount, int AdminCount);

/// <summary>A provisioned user summary.</summary>
/// <param name="Id">The user id.</param>
/// <param name="Email">The login email.</param>
/// <param name="Role">The user role.</param>
/// <param name="IsActive">Whether the user is active.</param>
/// <param name="CreatedAt">When the user was created (UTC).</param>
public sealed record ProvisionedUserDto(
    Guid Id, string Email, string Role, bool IsActive, DateTime CreatedAt);

/// <summary>A provisioned site summary.</summary>
/// <param name="Id">The site id.</param>
/// <param name="Name">The site name.</param>
/// <param name="Location">The site location, if any.</param>
/// <param name="IsActive">Whether the site is active.</param>
public sealed record ProvisionedSiteDto(
    Guid Id, string Name, string? Location, bool IsActive);
