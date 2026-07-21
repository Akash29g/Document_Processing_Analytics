using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta
using DocAnalytics.Service.ActivityLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Activity Log endpoint: paginated, filtered, chronological audit trail (FR-4).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/activity-log")]
[Tags("ActivityLog")]
public sealed class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _service;

    /// <summary>Creates a new <see cref="ActivityLogController"/>.</summary>
    /// <param name="service">Activity log query service.</param>
    public ActivityLogController(IActivityLogService service) => _service = service;

    /// <summary>Returns the paginated, filtered audit trail for the selected tenant/site (FR-4.1–FR-4.4).</summary>
    /// <param name="query">Filter (event type/entity/date range) and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of activity-log entries with metadata.</returns>
    /// <response code="200">Activity log returned.</response>
    // GET /api/v1/activity-log — paginated, filtered audit trail (FR-4.1–FR-4.4)
    [HttpGet]
    public async Task<IActionResult> GetActivityLog([FromQuery] ActivityLogQuery query, CancellationToken ct)
    {
        var result = await _service.GetActivityLogAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<ActivityLogItemDto>>.OkList(result.Items, meta));
    }
}
