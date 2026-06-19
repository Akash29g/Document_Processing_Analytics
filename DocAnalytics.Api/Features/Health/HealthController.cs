using DocAnalytics.Api.Common;
using DocAnalytics.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Features.Health;

[ApiController]
[Route("api/v1/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly IHealthService _health;
    public HealthController(IHealthService health) => _health = health;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ok = await _health.IsDatabaseReachableAsync();
        if (!ok) return StatusCode(503, ApiResponse<object>.Fail("DB_UNREACHABLE", "Database is unreachable"));
        return Ok(ApiResponse<object>.Ok(new { status = "healthy", db = "connected" }));
    }
}
