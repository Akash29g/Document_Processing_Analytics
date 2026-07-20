using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/errors")]
[Tags("Errors")]
[EnableRateLimiting("reads")]
public sealed class ErrorAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _chartService;
    public ErrorAnalyticsController(IAnalyticsService chartService) => _chartService = chartService;

    // GET /api/v1/errors/top-frequencies?topN=5
    [HttpGet("top-frequencies")]
    public async Task<IActionResult> GetTopErrors([FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var series = await _chartService.GetTopErrorsAsync(topN, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    // GET /api/v1/errors/trend?from=...&to=...
    [HttpGet("trend")]
    public async Task<IActionResult> GetErrorTrend([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _chartService.GetErrorTrendAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

}
