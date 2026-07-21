using DocAnalytics.Api.Common;
using DocAnalytics.Service.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>Dashboard endpoints: status summary counters and the recent-failures feed.</summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    /// <summary>Returns the dashboard status counters (queued, in-progress, completed, failed, total).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The dashboard summary payload.</returns>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var data = await _dashboardService.GetSummaryAsync(ct);
        return Ok(ApiResponse<DashboardSummaryResponse>.Ok(data));
    }

    /// <summary>Returns a paged list of recent failed steps for the current tenant/site.</summary>
    /// <param name="query">Paging and sort parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged list of recent failures with paging metadata.</returns>
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
