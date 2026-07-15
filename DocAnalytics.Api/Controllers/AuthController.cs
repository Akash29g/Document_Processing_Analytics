using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;   // ← NEW

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;
    private readonly ILoginLockoutService _lockout;   // ← NEW

    public AuthController(IAuthService auth, ICurrentUser currentUser, ILoginLockoutService lockout)
    {
        _auth = auth;
        _currentUser = currentUser;
        _lockout = lockout;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("login")]          // ← IP throttle (Step 7)
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var email = req.Email ?? string.Empty;

        // 1) Account-level lockout (survives restarts / spans multiple IPs).
        var (locked, retryAfter) = await _lockout.IsLockedAsync(email, ct);
        if (locked)
        {
            Response.Headers.RetryAfter = retryAfter.ToString();
            return StatusCode(StatusCodes.Status429TooManyRequests, ApiResponse<object>.Fail(
                "RATE_LIMITED", "Too many login attempts. Please try again later."));
        }

        // 2) Verify credentials.
        var result = await _auth.LoginAsync(req, ct);
        if (result is null)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _lockout.RegisterFailureAsync(email, ip, ct);   // count the miss (unknown emails too → no enumeration)
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_CREDENTIALS", "Email or password is incorrect."));
        }

        // 3) Success → clear the counter.
        await _lockout.ResetAsync(email, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
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
