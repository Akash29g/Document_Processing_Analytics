using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta
using DocAnalytics.Service.ActivityLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/activity-log")]
[Tags("ActivityLog")]
public sealed class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _service;
    public ActivityLogController(IActivityLogService service) => _service = service;

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
