using DocAnalytics.Api.Common;            // ApiResponse<T>
using DocAnalytics.Domain.Common;         // ICurrentUser
using DocAnalytics.Service.Alerts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly IAlertRuleService _svc;
    private readonly ICurrentUser _me;

    public AlertsController(IAlertRuleService svc, ICurrentUser me)
    {
        _svc = svc;
        _me = me;
    }

    // any authenticated user of this site can view
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<AlertRuleDto>>.Ok(await _svc.ListAsync(ct)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetAsync(id, ct);
        return dto is null
            ? NotFound(ApiResponse<AlertRuleDto>.Fail("NOT_FOUND", "Alert rule not found."))
            : Ok(ApiResponse<AlertRuleDto>.Ok(dto));
    }

    // ── writes: Admin only (S-3 RBAC) ──
    [HttpPost]
    public async Task<IActionResult> Create(CreateAlertRuleRequest req, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        return Ok(ApiResponse<AlertRuleDto>.Ok(await _svc.CreateAsync(req, ct)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAlertRuleRequest req, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        var dto = await _svc.UpdateAsync(id, req, ct);
        return dto is null
            ? NotFound(ApiResponse<AlertRuleDto>.Fail("NOT_FOUND", "Alert rule not found."))
            : Ok(ApiResponse<AlertRuleDto>.Ok(dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        return await _svc.DeleteAsync(id, ct)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Alert rule not found."));
    }

    private bool IsAdmin =>
        string.Equals(User.FindFirst("role")?.Value, "Admin", StringComparison.OrdinalIgnoreCase)
     || string.Equals(User.FindFirst(ClaimTypes.Role)?.Value, "Admin", StringComparison.OrdinalIgnoreCase);
    private IActionResult Forbidden() =>
        StatusCode(403, ApiResponse<object>.Fail("FORBIDDEN", "Admin role required."));
}
