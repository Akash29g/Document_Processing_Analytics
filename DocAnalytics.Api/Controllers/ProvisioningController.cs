using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Provisioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/provisioning")]
[Authorize(Policy = "DeveloperOnly")]
public sealed class ProvisioningController : ControllerBase
{
    private readonly IProvisioningService _service;
    private readonly ICurrentUser _currentUser;

    public ProvisioningController(IProvisioningService service, ICurrentUser currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    // ── tenants ──
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants(CancellationToken ct) =>
        Ok(ApiResponse<List<TenantSummaryDto>>.Ok(await _service.GetTenantsAsync(ct)));

    [HttpPost("tenants")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest req, CancellationToken ct)
    {
        var tenant = await _service.CreateTenantAsync(req, ct);
        if (tenant is null)
            return Conflict(ApiResponse<object>.Fail("DOMAIN_TAKEN", "A tenant with this org domain already exists."));
        return Ok(ApiResponse<TenantSummaryDto>.Ok(tenant));
    }

    // ── admins / users ──
    [HttpGet("tenants/{tenantId:guid}/users")]
    public async Task<IActionResult> GetUsers(Guid tenantId, CancellationToken ct) =>
        Ok(ApiResponse<List<ProvisionedUserDto>>.Ok(await _service.GetUsersAsync(tenantId, ct)));

    [HttpPost("tenants/{tenantId:guid}/admins")]
    public async Task<IActionResult> CreateAdmin(Guid tenantId, [FromBody] CreateAdminRequest req, CancellationToken ct)
    {
        var admin = await _service.CreateAdminAsync(tenantId, req, _currentUser.UserId, ct);
        if (admin is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Tenant not found or inactive."));
        return Ok(ApiResponse<ProvisionedUserDto>.Ok(admin));
    }

    [HttpDelete("tenants/{tenantId:guid}/admins/{userId:guid}")]
    public async Task<IActionResult> RemoveAdmin(Guid tenantId, Guid userId, CancellationToken ct)
    {
        if (!await _service.RemoveAdminAsync(tenantId, userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Admin not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    [HttpDelete("tenants/{tenantId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid tenantId, Guid userId, CancellationToken ct)
    {
        if (!await _service.RemoveUserAsync(tenantId, userId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "User not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }

    // ── sites ──
    [HttpGet("tenants/{tenantId:guid}/sites")]
    public async Task<IActionResult> GetSites(Guid tenantId, CancellationToken ct) =>
        Ok(ApiResponse<List<ProvisionedSiteDto>>.Ok(await _service.GetSitesAsync(tenantId, ct)));

    [HttpPost("tenants/{tenantId:guid}/sites")]
    public async Task<IActionResult> CreateSite(Guid tenantId, [FromBody] CreateSiteRequest req, CancellationToken ct)
    {
        var site = await _service.CreateSiteAsync(tenantId, req, ct);
        if (site is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Tenant not found or inactive."));
        return Ok(ApiResponse<ProvisionedSiteDto>.Ok(site));
    }

    [HttpDelete("tenants/{tenantId:guid}/sites/{siteId:guid}")]
    public async Task<IActionResult> RemoveSite(Guid tenantId, Guid siteId, CancellationToken ct)
    {
        if (!await _service.RemoveSiteAsync(tenantId, siteId, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Site not found in this tenant."));
        return Ok(ApiResponse<object>.Ok(new { removed = true }));
    }
}
