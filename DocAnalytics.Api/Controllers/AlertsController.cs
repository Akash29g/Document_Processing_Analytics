using System.Security.Claims;
using DocAnalytics.Api.Common;            // ApiResponse<T>
using DocAnalytics.Domain.Common;         // ICurrentUser
using DocAnalytics.Service.Alerts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Alert rule and in-app notification endpoints. Reads are open to any site user;
/// rule writes require the Admin role (S-3/S-4).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly IAlertRuleService _svc;
    private readonly ICurrentUser _me;
    private readonly IAlertNotificationService _notifications;

    /// <summary>Creates a new <see cref="AlertsController"/>.</summary>
    /// <param name="svc">Alert rule service.</param>
    /// <param name="me">The current authenticated user.</param>
    /// <param name="notifications">In-app alert notification service.</param>
    public AlertsController(IAlertRuleService svc, ICurrentUser me, IAlertNotificationService notifications)
    {
        _svc = svc;
        _me = me;
        _notifications = notifications;
    }

    /// <summary>Lists alert rules visible to the current user (Admins see all; Viewers see rules they're a recipient of).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The visible alert rules.</returns>
    /// <response code="200">Rules returned.</response>
    // any authenticated user of this site can view
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<AlertRuleDto>>.Ok(await _svc.ListAsync(ct)));

    /// <summary>Gets a single alert rule by id.</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The rule, or a not-found envelope.</returns>
    /// <response code="200">Rule found.</response>
    /// <response code="404">Rule not found or not visible to the caller.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetAsync(id, ct);
        return dto is null
            ? NotFound(ApiResponse<AlertRuleDto>.Fail("NOT_FOUND", "Alert rule not found."))
            : Ok(ApiResponse<AlertRuleDto>.Ok(dto));
    }

    /// <summary>Lists candidate recipients (active users) for the current site.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The eligible recipients.</returns>
    /// <response code="200">Recipients returned.</response>
    [HttpGet("recipients")]
    public async Task<IActionResult> Recipients(CancellationToken ct) =>
    Ok(ApiResponse<IReadOnlyList<RecipientDto>>.Ok(await _svc.ListRecipientsAsync(ct)));


    // ── writes: Admin only (S-3 RBAC) ──

    /// <summary>Creates a new alert rule (Admin only).</summary>
    /// <param name="req">The rule definition.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created rule.</returns>
    /// <response code="200">Rule created.</response>
    /// <response code="403">Caller is not an Admin.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateAlertRuleRequest req, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        return Ok(ApiResponse<AlertRuleDto>.Ok(await _svc.CreateAsync(req, ct)));
    }

    /// <summary>Updates an existing alert rule (Admin only).</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="req">The updated rule fields.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated rule, or a not-found envelope.</returns>
    /// <response code="200">Rule updated.</response>
    /// <response code="403">Caller is not an Admin.</response>
    /// <response code="404">Rule not found.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAlertRuleRequest req, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        var dto = await _svc.UpdateAsync(id, req, ct);
        return dto is null
            ? NotFound(ApiResponse<AlertRuleDto>.Fail("NOT_FOUND", "Alert rule not found."))
            : Ok(ApiResponse<AlertRuleDto>.Ok(dto));
    }

    /// <summary>Deletes an alert rule (Admin only).</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success, or a not-found envelope.</returns>
    /// <response code="204">Rule deleted.</response>
    /// <response code="403">Caller is not an Admin.</response>
    /// <response code="404">Rule not found.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!IsAdmin) return Forbidden();
        return await _svc.DeleteAsync(id, ct)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Alert rule not found."));
    }

    /// <summary>Returns in-app alert notifications for the current tenant/site.</summary>
    /// <param name="unread">When <c>true</c>, returns only unread notifications.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The notifications (most recent first).</returns>
    /// <response code="200">Notifications returned.</response>
    // GET /api/v1/alerts/notifications?unread=true
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unread = false, CancellationToken ct = default)
    {
        var list = await _notifications.GetNotificationsAsync(unread, ct);
        return Ok(ApiResponse<List<AlertNotificationDto>>.Ok(list));
    }

    /// <summary>Marks a single notification as read.</summary>
    /// <param name="id">The notification id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">Notification marked read.</response>
    /// <response code="404">Notification not found.</response>
    // POST /api/v1/alerts/notifications/{id}/read
    [HttpPost("notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        if (!await _notifications.MarkReadAsync(id, ct))
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Notification not found."));
        return Ok(ApiResponse<object>.Ok(new { read = true }));
    }

    /// <summary>Marks all notifications for the current tenant/site as read.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of notifications marked read.</returns>
    /// <response code="200">Notifications marked read.</response>
    // POST /api/v1/alerts/notifications/read-all
    [HttpPost("notifications/read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var count = await _notifications.MarkAllReadAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { marked = count }));
    }

    /// <summary>True when the current user carries the Admin role claim.</summary>
    private bool IsAdmin =>
        string.Equals(User.FindFirst("role")?.Value, "Admin", StringComparison.OrdinalIgnoreCase)
     || string.Equals(User.FindFirst(ClaimTypes.Role)?.Value, "Admin", StringComparison.OrdinalIgnoreCase);

    /// <summary>Builds a standard 403 Forbidden envelope for non-Admin write attempts.</summary>
    private IActionResult Forbidden() =>
        StatusCode(403, ApiResponse<object>.Fail("FORBIDDEN", "Admin role required."));
}
