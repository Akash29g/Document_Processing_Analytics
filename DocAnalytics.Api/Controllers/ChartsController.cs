using DocAnalytics.Api.Common;
using DocAnalytics.Service.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/charts")]
public sealed class ChartsController : ControllerBase
{
    private readonly IChartService _chartService;
    public ChartsController(IChartService chartService) => _chartService = chartService;

    // GET /api/v1/charts/status-distribution
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken ct)
    {
        var series = await _chartService.GetStatusDistributionAsync(ct);
        return Ok(ApiResponse<ChartSeriesDto>.Ok(series));
    }

    // GET /api/v1/charts/throughput
    [HttpGet("throughput")]
    public async Task<IActionResult> GetThroughput(CancellationToken ct)
    {
        var series = await _chartService.GetThroughputAsync(ct);
        return Ok(ApiResponse<ChartSeriesDto>.Ok(series));
    }


}
