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
    private readonly IRefreshTokenService _refresh;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordResetService _passwordReset;

    /// <summary>Creates a new <see cref="AuthController"/>.</summary>
    public AuthController(
        IAuthService auth,
        ICurrentUser currentUser,
        ILoginLockoutService lockout,
        IRefreshTokenService refresh,
        IJwtTokenService jwt,
        IPasswordResetService passwordReset) 
    {
        _auth = auth;
        _currentUser = currentUser;
        _lockout = lockout;
        _refresh = refresh;
        _jwt = jwt;
        _passwordReset = passwordReset;
    }

    /// <summary>Authenticates a user and issues a JWT access token; the refresh token is set as an HttpOnly cookie.</summary>
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

        // Refresh token now lives ONLY in an HttpOnly cookie — never in the JSON body.
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var (raw, refreshExpiresAt) = await _refresh.IssueAsync(result.User.Id, clientIp, ct);
        SetRefreshCookie(raw, refreshExpiresAt);

        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>Exchanges the refresh-token cookie for a fresh access token and rotates the cookie.</summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var presented = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(presented))
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_REFRESH_TOKEN", "Refresh token is missing."));

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var rotated = await _refresh.ValidateAndRotateAsync(presented, ip, ct);
        if (rotated is null)
        {
            DeleteRefreshCookie();   // clear the bad cookie
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_REFRESH_TOKEN", "Refresh token is invalid or expired."));
        }

        var (user, newRaw, newExpiresAt) = rotated.Value;
        SetRefreshCookie(newRaw, newExpiresAt);   // rotate the cookie
        var accessToken = _jwt.CreateToken(user);
        return Ok(ApiResponse<RefreshResponse>.Ok(new RefreshResponse(accessToken)));
    }

    /// <summary>Revokes the refresh token (logout) and clears the cookie.</summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var raw = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(raw))
            await _refresh.RevokeAsync(raw, ct);

        DeleteRefreshCookie();
        return Ok(ApiResponse<object>.Ok(new { logged_out = true }));
    }

    /// <summary>Returns the current authenticated user's profile and authorized sites (session rehydration).</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var result = await _auth.GetMeAsync(_currentUser.UserId, ct);
        if (result is null) return Unauthorized();
        return Ok(ApiResponse<MeResponse>.Ok(result));
    }

    /// <summary>Changes the current user's password.</summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var error = await _auth.ChangePasswordAsync(_currentUser.UserId, req, ct);
        if (error is not null)
            return BadRequest(ApiResponse<object>.Fail("INVALID_PASSWORD", error));
        return Ok(ApiResponse<object>.Ok(new { changed = true }));
    }

    /// <summary>Starts the forgot-password flow. Always returns 200 — never reveals whether the email exists.</summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _passwordReset.RequestResetAsync(req, ip, ct);

        // Generic message on purpose (no account enumeration).
        return Ok(ApiResponse<object>.Ok(new
        {
            message = "If an account exists for that email, a reset link has been sent."
        }));
    }

    /// <summary>Completes the forgot-password flow: consumes a reset token and sets the new password.</summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req, CancellationToken ct)
    {
        var error = await _passwordReset.ResetPasswordAsync(req, ct);
        if (error is not null)
            return BadRequest(ApiResponse<object>.Fail("INVALID_RESET", error));

        return Ok(ApiResponse<object>.Ok(new { reset = true }));
    }


    // ── refresh-token cookie helpers ────────────────────────────────────────
    private void SetRefreshCookie(string rawToken, DateTime expiresAt)
    {
        Response.Cookies.Append("refresh_token", rawToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
            Path = "/api/v1/auth"
        });
    }

    private void DeleteRefreshCookie()
    {
        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth"
        });
    }
}
