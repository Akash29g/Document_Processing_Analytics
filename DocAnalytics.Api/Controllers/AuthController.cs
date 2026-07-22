using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Authentication endpoints: login, refresh-token rotation, logout, current-user, and password change.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;
    private readonly ILoginLockoutService _lockout;
    private readonly IRefreshTokenService _refresh;   // ← NEW (R4)
    private readonly IJwtTokenService _jwt;            // ← NEW (R4)

    /// <summary>Creates a new <see cref="AuthController"/>.</summary>
    /// <param name="auth">Authentication service.</param>
    /// <param name="currentUser">The current authenticated user.</param>
    /// <param name="lockout">Login lockout (brute-force) service.</param>
    /// <param name="refresh">Refresh-token service.</param>
    /// <param name="jwt">JWT access-token service.</param>
    public AuthController(
        IAuthService auth,
        ICurrentUser currentUser,
        ILoginLockoutService lockout,
        IRefreshTokenService refresh,
        IJwtTokenService jwt)
    {
        _auth = auth;
        _currentUser = currentUser;
        _lockout = lockout;
        _refresh = refresh;
        _jwt = jwt;
    }

    /// <summary>Authenticates a user and issues a JWT access token plus a rotating refresh token.</summary>
    /// <param name="req">Login request with email and password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The access token, refresh token, and the user's authorized sites.</returns>
    /// <response code="200">Login succeeded.</response>
    /// <response code="401">Email or password is incorrect.</response>
    /// <response code="429">Too many attempts — rate limited or account locked.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var email = req.Email ?? string.Empty;

        var (locked, retryAfter) = await _lockout.IsLockedAsync(email, ct);
        if (locked)
        {
            Response.Headers.RetryAfter = retryAfter.ToString();
            return StatusCode(StatusCodes.Status429TooManyRequests, ApiResponse<object>.Fail(
                "RATE_LIMITED", "Too many login attempts. Please try again later."));
        }

        var result = await _auth.LoginAsync(req, ct);
        if (result is null)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _lockout.RegisterFailureAsync(email, ip, ct);
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_CREDENTIALS", "Email or password is incorrect."));
        }

        await _lockout.ResetAsync(email, ct);

        // NEW (R4): mint a rotating refresh token alongside the 15-min access token.
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var (raw, _) = await _refresh.IssueAsync(result.User.Id, clientIp, ct);
        result = result with { RefreshToken = raw };

        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>Exchanges a valid refresh token for a fresh access token and a rotated refresh token.</summary>
    /// <param name="req">Request containing the current refresh token.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A new access token and rotated refresh token.</returns>
    /// <response code="200">New tokens issued.</response>
    /// <response code="401">Refresh token is invalid or expired.</response>
    // NEW (R4): exchange a valid refresh token for a fresh access token + rotated refresh token.
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var rotated = await _refresh.ValidateAndRotateAsync(req.RefreshToken, ip, ct);
        if (rotated is null)
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_REFRESH_TOKEN", "Refresh token is invalid or expired."));

        var (user, newRaw, _) = rotated.Value;
        var accessToken = _jwt.CreateToken(user);
        return Ok(ApiResponse<RefreshResponse>.Ok(new RefreshResponse(accessToken, newRaw)));
    }

    /// <summary>Revokes a refresh token (logout). Anonymous so an expired access token can't block cleanup.</summary>
    /// <param name="req">Request containing the refresh token to revoke.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Confirmation that the token was revoked (idempotent).</returns>
    /// <response code="200">Token revoked.</response>
    // NEW (R4): revoke a refresh token (logout). AllowAnonymous so an expired
    // access token doesn't block the client from cleanly revoking.
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest req, CancellationToken ct)
    {
        await _refresh.RevokeAsync(req.RefreshToken, ct);
        return Ok(ApiResponse<object>.Ok(new { logged_out = true }));
    }

    /// <summary>Returns the current authenticated user's profile and authorized sites (session rehydration).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The current user's profile, or 401 if no valid session.</returns>
    /// <response code="200">Profile returned.</response>
    /// <response code="401">No valid session.</response>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var result = await _auth.GetMeAsync(_currentUser.UserId, ct);
        if (result is null) return Unauthorized();
        return Ok(ApiResponse<MeResponse>.Ok(result));
    }

    /// <summary>Changes the current user's password.</summary>
    /// <param name="req">Current and new password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Confirmation, or a validation error.</returns>
    /// <response code="200">Password changed.</response>
    /// <response code="400">Current password is incorrect, or the new password fails the policy / breach check.</response>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var error = await _auth.ChangePasswordAsync(_currentUser.UserId, req, ct);
        if (error is not null)
            return BadRequest(ApiResponse<object>.Fail("INVALID_PASSWORD", error));
        return Ok(ApiResponse<object>.Ok(new { changed = true }));
    }

}
