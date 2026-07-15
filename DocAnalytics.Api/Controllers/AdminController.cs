using DocAnalytics.Api.Common;
using DocAnalytics.Service.AdminUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminController(IAdminUserService service) => _service = service;

    // ── users ──
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct) =>
        Ok(ApiResponse<List<AdminUserDto>>.Ok(await _service.GetUsersAsync(ct)));

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserRequest req, CancellationToken ct)
    {
        var created = await _service.CreateUserAsync(req, ct);
        if (created is null)
            return BadRequest(ApiResponse<object>.Fail(
                "INVALID_SITES", "One or more site ids do not belong to your company."));
        return Ok(ApiResponse<AdminCreatedUserDto>.Ok(created));
    }

    [HttpPut("users/{userId:guid}/sites")]
    public async Task<IActionResult> UpdateUserSites(Guid userId, [FromBody] UpdateUserSitesRequest req, CancellationToken ct)
    {
        if (!await _service.UpdateUserSitesAsync(userId, req, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in your company, or invalid site ids."));
        return Ok(ApiResponse<object>.Ok(new { updated = true }));
    }

    [HttpDelete("users/{userId:guid}")]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken ct)
    {
        if (!await _service.DeactivateUserAsync(userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in your company."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    // ── sites ──
    [HttpGet("sites")]
    public async Task<IActionResult> GetSites(CancellationToken ct) =>
        Ok(ApiResponse<List<AdminSiteDto>>.Ok(await _service.GetSitesAsync(ct)));

    [HttpPost("sites")]
    public async Task<IActionResult> CreateSite([FromBody] AdminCreateSiteRequest req, CancellationToken ct) =>
        Ok(ApiResponse<AdminSiteDto>.Ok(await _service.CreateSiteAsync(req, ct)));
}
