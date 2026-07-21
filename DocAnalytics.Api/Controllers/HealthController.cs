using DocAnalytics.Api.Common;
using DocAnalytics.Service.Health;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Health-check endpoint used by load balancers and uptime monitors (NFR-4). Unauthenticated.
/// </summary>
[ApiController]
[Route("api/v1/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly IHealthService _health;

    /// <summary>Creates a new <see cref="HealthController"/>.</summary>
    /// <param name="health">Health service that probes database connectivity.</param>
    public HealthController(IHealthService health) => _health = health;

    /// <summary>Reports service health by verifying database connectivity.</summary>
    /// <returns>A healthy status envelope, or a 503 when the database is unreachable.</returns>
    /// <response code="200">Service healthy; database connected.</response>
    /// <response code="503">Database is unreachable.</response>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ok = await _health.IsDatabaseReachableAsync();
        if (!ok) return StatusCode(503, ApiResponse<object>.Fail("DB_UNREACHABLE", "Database is unreachable"));
        return Ok(ApiResponse<object>.Ok(new { status = "healthy", db = "connected" }));
    }
}
