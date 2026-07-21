using DocAnalytics.Api.Common;
using DocAnalytics.Service.Realtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Development-only endpoint that simulates a pipeline state change to demo real-time (SignalR) updates.
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/dev")]
public sealed class SimulationController : ControllerBase
{
    private readonly ISimulationService _sim;
    private readonly IWebHostEnvironment _env;

    /// <summary>Creates a new <see cref="SimulationController"/>.</summary>
    /// <param name="sim">Simulation service.</param>
    /// <param name="env">Hosting environment (used to hide this endpoint outside Development).</param>
    public SimulationController(ISimulationService sim, IWebHostEnvironment env)
    {
        _sim = sim; _env = env;
    }

    /// <summary>Flips one file's state, updates counters, writes an audit row, and broadcasts the change. Development only.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The simulated state-change notification, or a not-found envelope.</returns>
    /// <response code="200">A state change was simulated and broadcast.</response>
    /// <response code="404">Not in Development, or no files available to simulate.</response>
    // POST /api/v1/dev/simulate-state-change  (Development only)
    [HttpPost("simulate-state-change")]
    public async Task<IActionResult> SimulateStateChange(CancellationToken ct)
    {
        if (!_env.IsDevelopment())
            return NotFound();   // hidden outside dev

        var result = await _sim.SimulateStateChangeAsync(ct);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("NO_FILES", "No files available to simulate."));

        return Ok(ApiResponse<FileStateChangedNotification>.Ok(result));
    }
}
