using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;
    private readonly ILoginLockoutService _lockout;
    private readonly IRefreshTokenService _refresh;   // ← NEW (R4)
    private readonly IJwtTokenService _jwt;            // ← NEW (R4)

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

    // NEW (R4): revoke a refresh token (logout). AllowAnonymous so an expired
    // access token doesn't block the client from cleanly revoking.
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest req, CancellationToken ct)
    {
        await _refresh.RevokeAsync(req.RefreshToken, ct);
        return Ok(ApiResponse<object>.Ok(new { logged_out = true }));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var result = await _auth.GetMeAsync(_currentUser.UserId, ct);
        if (result is null) return Unauthorized();
        return Ok(ApiResponse<MeResponse>.Ok(result));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var ok = await _auth.ChangePasswordAsync(_currentUser.UserId, req, ct);
        if (!ok)
            return BadRequest(ApiResponse<object>.Fail(
                "INVALID_PASSWORD", "Current password is incorrect."));
        return Ok(ApiResponse<object>.Ok(new { changed = true }));
    }
}
