using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Auth;

// What the client SENDS to POST /auth/login
public record LoginRequest(string Email, string Password);

// What POST /auth/login RETURNS
public record LoginResponse(string Token, UserDto User, IReadOnlyList<SiteDto> Sites, bool MustChangePassword)
{
    public string? RefreshToken { get; init; }   // NEW (R4) — serialized as refresh_token
}

// What the client SENDS to POST /auth/change-password
public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required, MinLength(10)] string NewPassword);

// What GET /auth/me RETURNS
public record MeResponse(UserDto User, IReadOnlyList<SiteDto> Sites);

// Safe view of a user — NOTE: no password hash ever leaves here
public record UserDto(Guid Id, string Email, string Role);

// One site the user is allowed to access
public record SiteDto(Guid SiteId, string SiteName);

// NEW (R4) — POST /auth/refresh
public record RefreshRequest(string RefreshToken);
public record RefreshResponse(string Token, string RefreshToken);

// NEW (R4) — POST /auth/logout
public record LogoutRequest(string RefreshToken);
