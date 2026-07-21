using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Auth;

/// <summary>Login request body for POST /auth/login.</summary>
/// <param name="Email">User email.</param>
/// <param name="Password">User password.</param>
// What the client SENDS to POST /auth/login
public record LoginRequest(string Email, string Password);

/// <summary>Login response for POST /auth/login.</summary>
/// <param name="Token">The JWT access token.</param>
/// <param name="User">The authenticated user (no secrets).</param>
/// <param name="Sites">The sites the user may access.</param>
/// <param name="MustChangePassword">Whether a forced first-login password change is required.</param>
// What POST /auth/login RETURNS
public record LoginResponse(string Token, UserDto User, IReadOnlyList<SiteDto> Sites, bool MustChangePassword)
{
    /// <summary>The rotating refresh token (serialized as refresh_token).</summary>
    public string? RefreshToken { get; init; }   // NEW (R4) — serialized as refresh_token
}

/// <summary>Change-password request body for POST /auth/change-password.</summary>
/// <param name="CurrentPassword">The current password (must be verified).</param>
/// <param name="NewPassword">The new password (min length 10).</param>
// What the client SENDS to POST /auth/change-password
public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required, MinLength(10)] string NewPassword);

/// <summary>Response for GET /auth/me: the current user and their sites.</summary>
/// <param name="User">The current user.</param>
/// <param name="Sites">The sites the user may access.</param>
// What GET /auth/me RETURNS
public record MeResponse(UserDto User, IReadOnlyList<SiteDto> Sites);

/// <summary>A safe view of a user — never includes the password hash.</summary>
/// <param name="Id">User id.</param>
/// <param name="Email">User email.</param>
/// <param name="Role">User role.</param>
// Safe view of a user — NOTE: no password hash ever leaves here
public record UserDto(Guid Id, string Email, string Role);

/// <summary>A site the user is allowed to access.</summary>
/// <param name="SiteId">Site id.</param>
/// <param name="SiteName">Site name.</param>
// One site the user is allowed to access
public record SiteDto(Guid SiteId, string SiteName);

/// <summary>Refresh request body for POST /auth/refresh.</summary>
/// <param name="RefreshToken">The current refresh token.</param>
// NEW (R4) — POST /auth/refresh
public record RefreshRequest(string RefreshToken);

/// <summary>Refresh response: a new access token and rotated refresh token.</summary>
/// <param name="Token">The new JWT access token.</param>
/// <param name="RefreshToken">The rotated refresh token.</param>
public record RefreshResponse(string Token, string RefreshToken);

/// <summary>Logout request body for POST /auth/logout.</summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
// NEW (R4) — POST /auth/logout
public record LogoutRequest(string RefreshToken);
