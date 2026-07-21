using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Error analysis chart endpoints: top error frequencies and the error trend series (FR-3.1, FR-3.2).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/errors")]
[Tags("Errors")]
[EnableRateLimiting("reads")]
public sealed class ErrorAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _chartService;

    /// <summary>Creates a new <see cref="ErrorAnalyticsController"/>.</summary>
    /// <param name="chartService">Analytics/aggregation service.</param>
    public ErrorAnalyticsController(IAnalyticsService chartService) => _chartService = chartService;

    /// <summary>Returns the top-N most frequent errors with remediation text (FR-3.1).</summary>
    /// <param name="topN">Number of top errors to return (default 5).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A labelled series of error codes and counts.</returns>
    /// <response code="200">Top errors returned.</response>
    // GET /api/v1/errors/top-frequencies?topN=5
    [HttpGet("top-frequencies")]
    public async Task<IActionResult> GetTopErrors([FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var series = await _chartService.GetTopErrorsAsync(topN, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    /// <summary>Returns the number of failures per day over an optional date range (FR-3.2).</summary>
    /// <param name="query">Optional <c>from</c>/<c>to</c> date range.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A time-bucketed series of failure counts.</returns>
    /// <response code="200">Error trend returned.</response>
    // GET /api/v1/errors/trend?from=...&to=...
    [HttpGet("trend")]
    public async Task<IActionResult> GetErrorTrend([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _chartService.GetErrorTrendAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

}
