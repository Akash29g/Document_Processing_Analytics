using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/dashboard")]
[Tags("Dashboard")]
[EnableRateLimiting("reads")]
public sealed class DashboardAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    public DashboardAnalyticsController(IAnalyticsService chartService) => _analyticsService = chartService;

    // GET /api/v1/dashboard/status-distribution
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken ct)
    {
        var series = await _analyticsService.GetStatusDistributionAsync(ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    // GET /api/v1/dashboard/throughput?from=...&to=...
    [HttpGet("throughput")]
    public async Task<IActionResult> GetThroughput([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _analyticsService.GetThroughputAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    // GET /api/v1/dashboard/step-percentiles  (S-5)
    [HttpGet("step-percentiles")]
    public async Task<IActionResult> GetStepPercentiles(CancellationToken ct)
    {
        var data = await _analyticsService.GetStepPercentilesAsync(ct);
        return Ok(ApiResponse<List<StepPercentileDto>>.Ok(data));
    }




}
