using DocAnalytics.Api.Common;
using DocAnalytics.Service.AdminUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Tenant-admin endpoints for managing Viewer users and sites within the caller's own tenant (AdminOnly).
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminUserService _service;

    /// <summary>Creates a new <see cref="AdminController"/>.</summary>
    /// <param name="service">Admin user/site management service.</param>
    public AdminController(IAdminUserService service) => _service = service;

    // ── users ──

    /// <summary>Lists the Viewer users in the caller's tenant.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's users.</returns>
    /// <response code="200">Users returned.</response>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct) =>
        Ok(ApiResponse<List<AdminUserDto>>.Ok(await _service.GetUsersAsync(ct)));

    /// <summary>Creates a Viewer user and emails generated credentials.</summary>
    /// <param name="req">First/last name and the site ids to grant.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user, or a validation error.</returns>
    /// <response code="200">User created.</response>
    /// <response code="400">One or more site ids do not belong to the caller's tenant.</response>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserRequest req, CancellationToken ct)
    {
        var created = await _service.CreateUserAsync(req, ct);
        if (created is null)
            return BadRequest(ApiResponse<object>.Fail(
                "INVALID_SITES", "One or more site ids do not belong to your company."));
        return Ok(ApiResponse<AdminCreatedUserDto>.Ok(created));
    }

    /// <summary>Replaces the set of sites a user can access.</summary>
    /// <param name="userId">The target user id.</param>
    /// <param name="req">The new set of site ids.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found/validation error.</returns>
    /// <response code="200">Sites updated.</response>
    /// <response code="404">User not found in the caller's tenant, or invalid site ids.</response>
    [HttpPut("users/{userId:guid}/sites")]
    public async Task<IActionResult> UpdateUserSites(Guid userId, [FromBody] UpdateUserSitesRequest req, CancellationToken ct)
    {
        if (!await _service.UpdateUserSitesAsync(userId, req, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in your company, or invalid site ids."));
        return Ok(ApiResponse<object>.Ok(new { updated = true }));
    }

    /// <summary>Deactivates (soft-deletes) a Viewer user in the caller's tenant.</summary>
    /// <param name="userId">The target user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found error.</returns>
    /// <response code="200">User deactivated.</response>
    /// <response code="404">User not found in the caller's tenant.</response>
    [HttpDelete("users/{userId:guid}")]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken ct)
    {
        if (!await _service.DeactivateUserAsync(userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in your company."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    // ── sites ──

    /// <summary>Lists the sites in the caller's tenant.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's sites.</returns>
    /// <response code="200">Sites returned.</response>
    [HttpGet("sites")]
    public async Task<IActionResult> GetSites(CancellationToken ct) =>
        Ok(ApiResponse<List<AdminSiteDto>>.Ok(await _service.GetSitesAsync(ct)));

    /// <summary>Creates a new site in the caller's tenant and grants the creator access.</summary>
    /// <param name="req">Site name and location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created site.</returns>
    /// <response code="200">Site created.</response>
    [HttpPost("sites")]
    public async Task<IActionResult> CreateSite([FromBody] AdminCreateSiteRequest req, CancellationToken ct) =>
        Ok(ApiResponse<AdminSiteDto>.Ok(await _service.CreateSiteAsync(req, ct)));
}
