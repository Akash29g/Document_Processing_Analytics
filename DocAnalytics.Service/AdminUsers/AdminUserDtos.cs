using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.AdminUsers;

// ── requests ──
public sealed record AdminCreateUserRequest(
    [Required, MaxLength(60)] string FirstName,
    [Required, MaxLength(60)] string LastName,
    [Required, MinLength(1)] List<Guid> SiteIds);

public sealed record AdminCreateSiteRequest(
    [Required, MaxLength(120)] string Name,
    [MaxLength(120)] string? Location);

public sealed record UpdateUserSitesRequest(
    [Required] List<Guid> SiteIds);

// ── responses ──
public sealed record AdminUserDto(
    Guid Id, string Email, string Role, bool IsActive,
    DateTime CreatedAt, List<Guid> SiteIds);

public sealed record AdminSiteDto(Guid Id, string Name, string? Location, bool IsActive);

public sealed record AdminCreatedUserDto(Guid Id, string Email, bool CredentialsEmailed);
