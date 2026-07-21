using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Dashboard chart endpoints: status distribution, throughput, and per-step
/// processing-time percentiles (FR-1.2, FR-1.3, S-5).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/dashboard")]
[Tags("Dashboard")]
[EnableRateLimiting("reads")]
public sealed class DashboardAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    /// <summary>Creates a new <see cref="DashboardAnalyticsController"/>.</summary>
    /// <param name="chartService">Analytics/aggregation service.</param>
    public DashboardAnalyticsController(IAnalyticsService chartService) => _analyticsService = chartService;

    /// <summary>Returns the current file status distribution for the pie/bar chart (FR-1.3).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A labelled series of status counts.</returns>
    /// <response code="200">Distribution returned.</response>
    // GET /api/v1/dashboard/status-distribution
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken ct)
    {
        var series = await _analyticsService.GetStatusDistributionAsync(ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    /// <summary>Returns processing throughput over an optional date range (FR-1.2).</summary>
    /// <param name="query">Optional <c>from</c>/<c>to</c> date range.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A time-bucketed series of completed-file counts.</returns>
    /// <response code="200">Throughput series returned.</response>
    // GET /api/v1/dashboard/throughput?from=...&to=...
    [HttpGet("throughput")]
    public async Task<IActionResult> GetThroughput([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _analyticsService.GetThroughputAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    /// <summary>Returns P50/P90/P99 processing-time percentiles per pipeline step (S-5).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of per-step percentile rows.</returns>
    /// <response code="200">Percentiles returned.</response>
    // GET /api/v1/dashboard/step-percentiles  (S-5)
    [HttpGet("step-percentiles")]
    public async Task<IActionResult> GetStepPercentiles(CancellationToken ct)
    {
        var data = await _analyticsService.GetStepPercentilesAsync(ct);
        return Ok(ApiResponse<List<StepPercentileDto>>.Ok(data));
    }




}
