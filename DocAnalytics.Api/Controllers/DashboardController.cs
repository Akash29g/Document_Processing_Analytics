using DocAnalytics.Api.Common;
using DocAnalytics.Service.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var data = await _dashboardService.GetSummaryAsync(ct);
        return Ok(ApiResponse<DashboardSummaryResponse>.Ok(data));
    }

    [HttpGet("recent-failures")]
    public async Task<IActionResult> GetRecentFailures(
        [FromQuery] RecentFailuresQuery query, CancellationToken ct)
    {
        var result = await _dashboardService.GetRecentFailuresAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<RecentFailureDto>>.OkList(result.Items, meta));
    }
}
