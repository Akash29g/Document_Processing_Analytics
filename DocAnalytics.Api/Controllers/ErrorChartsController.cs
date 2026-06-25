using DocAnalytics.Api.Common;
using DocAnalytics.Service.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/errors")]
public sealed class ErrorChartsController : ControllerBase
{
    private readonly IChartService _chartService;
    public ErrorChartsController(IChartService chartService) => _chartService = chartService;

    // GET /api/v1/errors/top-frequencies?topN=5
    [HttpGet("top-frequencies")]
    public async Task<IActionResult> GetTopErrors([FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var series = await _chartService.GetTopErrorsAsync(topN, ct);
        return Ok(ApiResponse<ChartSeriesDto>.Ok(series));
    }

    // GET /api/v1/errors/trend
    [HttpGet("trend")]
    public async Task<IActionResult> GetErrorTrend(CancellationToken ct)
    {
        var series = await _chartService.GetErrorTrendAsync(ct);
        return Ok(ApiResponse<ChartSeriesDto>.Ok(series));
    }
}
