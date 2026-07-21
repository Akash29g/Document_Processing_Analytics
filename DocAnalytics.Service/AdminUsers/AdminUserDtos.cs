using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.AdminUsers;

// ── requests ──

/// <summary>Request to create a Viewer user and grant them the given sites.</summary>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="SiteIds">Ids of the sites to grant access to.</param>
public sealed record AdminCreateUserRequest(
    [Required, MaxLength(60)] string FirstName,
    [Required, MaxLength(60)] string LastName,
    [Required, MinLength(1)] List<Guid> SiteIds);

/// <summary>Request to create a new site in the caller's tenant.</summary>
/// <param name="Name">Site name.</param>
/// <param name="Location">Optional site location.</param>
public sealed record AdminCreateSiteRequest(
    [Required, MaxLength(120)] string Name,
    [MaxLength(120)] string? Location);

/// <summary>Request to replace the set of sites a user may access.</summary>
/// <param name="SiteIds">The new set of site ids.</param>
public sealed record UpdateUserSitesRequest(
    [Required] List<Guid> SiteIds);

// ── responses ──

/// <summary>An admin-managed user with their granted sites.</summary>
/// <param name="Id">User id.</param>
/// <param name="Email">User email (login).</param>
/// <param name="Role">User role.</param>
/// <param name="IsActive">Whether the account is active.</param>
/// <param name="CreatedAt">Creation timestamp (UTC).</param>
/// <param name="SiteIds">Ids of the sites the user can access.</param>
public sealed record AdminUserDto(
    Guid Id, string Email, string Role, bool IsActive,
    DateTime CreatedAt, List<Guid> SiteIds);

/// <summary>A site within a tenant.</summary>
/// <param name="Id">Site id.</param>
/// <param name="Name">Site name.</param>
/// <param name="Location">Site location, if set.</param>
/// <param name="IsActive">Whether the site is active.</param>
public sealed record AdminSiteDto(Guid Id, string Name, string? Location, bool IsActive);

/// <summary>Result of creating a user, including whether credentials were emailed.</summary>
/// <param name="Id">New user id.</param>
/// <param name="Email">Generated login email.</param>
/// <param name="CredentialsEmailed">Whether the welcome/credentials email was sent.</param>
public sealed record AdminCreatedUserDto(Guid Id, string Email, bool CredentialsEmailed);
