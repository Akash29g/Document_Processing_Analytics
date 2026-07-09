using DocAnalytics.Api.Common;
using DocAnalytics.Service.Realtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/dev")]
public sealed class SimulationController : ControllerBase
{
    private readonly ISimulationService _sim;
    private readonly IWebHostEnvironment _env;

    public SimulationController(ISimulationService sim, IWebHostEnvironment env)
    {
        _sim = sim; _env = env;
    }

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
