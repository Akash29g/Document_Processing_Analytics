using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Provisioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Platform provisioning endpoints (Developer only): manage tenants, their admins/users, and sites.
/// </summary>
[ApiController]
[Route("api/v1/provisioning")]
[Authorize(Policy = "DeveloperOnly")]
public sealed class ProvisioningController : ControllerBase
{
    private readonly IProvisioningService _service;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates a new <see cref="ProvisioningController"/>.</summary>
    /// <param name="service">Provisioning service.</param>
    /// <param name="currentUser">The current authenticated (Developer) user.</param>
    public ProvisioningController(IProvisioningService service, ICurrentUser currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    // ── tenants ──

    /// <summary>Lists all tenants with summary counts.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>All tenants.</returns>
    /// <response code="200">Tenants returned.</response>
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants(CancellationToken ct) =>
        Ok(ApiResponse<List<TenantSummaryDto>>.Ok(await _service.GetTenantsAsync(ct)));

    /// <summary>Creates a new tenant.</summary>
    /// <param name="req">Tenant name and org domain.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created tenant, or a conflict if the domain is taken.</returns>
    /// <response code="200">Tenant created.</response>
    /// <response code="409">A tenant with this org domain already exists.</response>
    [HttpPost("tenants")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest req, CancellationToken ct)
    {
        var tenant = await _service.CreateTenantAsync(req, ct);
        if (tenant is null)
            return Conflict(ApiResponse<object>.Fail("DOMAIN_TAKEN", "A tenant with this org domain already exists."));
        return Ok(ApiResponse<TenantSummaryDto>.Ok(tenant));
    }

    // ── admins / users ──

    /// <summary>Lists the users belonging to a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's users.</returns>
    /// <response code="200">Users returned.</response>
    [HttpGet("tenants/{tenantId:guid}/users")]
    public async Task<IActionResult> GetUsers(Guid tenantId, CancellationToken ct) =>
        Ok(ApiResponse<List<ProvisionedUserDto>>.Ok(await _service.GetUsersAsync(tenantId, ct)));

    /// <summary>Creates an admin user in a tenant and emails generated credentials.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="req">First/last name for the admin.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created admin, or a not-found envelope.</returns>
    /// <response code="200">Admin created.</response>
    /// <response code="404">Tenant not found or inactive.</response>
    [HttpPost("tenants/{tenantId:guid}/admins")]
    public async Task<IActionResult> CreateAdmin(Guid tenantId, [FromBody] CreateAdminRequest req, CancellationToken ct)
    {
        var admin = await _service.CreateAdminAsync(tenantId, req, _currentUser.UserId, ct);
        if (admin is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Tenant not found or inactive."));
        return Ok(ApiResponse<ProvisionedUserDto>.Ok(admin));
    }

    /// <summary>Removes (deactivates) an admin from a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="userId">The admin user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">Admin removed.</response>
    /// <response code="404">Admin not found in this tenant.</response>
    [HttpDelete("tenants/{tenantId:guid}/admins/{userId:guid}")]
    public async Task<IActionResult> RemoveAdmin(Guid tenantId, Guid userId, CancellationToken ct)
    {
        if (!await _service.RemoveAdminAsync(tenantId, userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Admin not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    /// <summary>Removes (deactivates) a user from a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">User removed.</response>
    /// <response code="404">User not found in this tenant.</response>
    [HttpDelete("tenants/{tenantId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid tenantId, Guid userId, CancellationToken ct)
    {
        if (!await _service.RemoveUserAsync(tenantId, userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    // ── sites ──

    /// <summary>Lists the sites belonging to a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's sites.</returns>
    /// <response code="200">Sites returned.</response>
    [HttpGet("tenants/{tenantId:guid}/sites")]
    public async Task<IActionResult> GetSites(Guid tenantId, CancellationToken ct) =>
        Ok(ApiResponse<List<ProvisionedSiteDto>>.Ok(await _service.GetSitesAsync(tenantId, ct)));

    /// <summary>Creates a new site in a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="req">Site name and location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created site, or a not-found envelope.</returns>
    /// <response code="200">Site created.</response>
    /// <response code="404">Tenant not found or inactive.</response>
    [HttpPost("tenants/{tenantId:guid}/sites")]
    public async Task<IActionResult> CreateSite(Guid tenantId, [FromBody] CreateSiteRequest req, CancellationToken ct)
    {
        var site = await _service.CreateSiteAsync(tenantId, req, ct);
        if (site is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Tenant not found or inactive."));
        return Ok(ApiResponse<ProvisionedSiteDto>.Ok(site));
    }

    /// <summary>Removes (deactivates) a site from a tenant and revokes access to it.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="siteId">The site id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">Site removed.</response>
    /// <response code="404">Site not found in this tenant.</response>
    [HttpDelete("tenants/{tenantId:guid}/sites/{siteId:guid}")]
    public async Task<IActionResult> RemoveSite(Guid tenantId, Guid siteId, CancellationToken ct)
    {
        if (!await _service.RemoveSiteAsync(tenantId, siteId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Site not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }
}
