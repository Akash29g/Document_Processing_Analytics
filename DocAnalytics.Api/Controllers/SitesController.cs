using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/sites")]                   // note: /sites, NOT /auth/sites (per DT-2)
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
public class SitesController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public SitesController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSites(CancellationToken ct)
    {
        var sites = await _auth.GetSitesAsync(_currentUser.UserId, ct);
        return Ok(ApiResponse<IReadOnlyList<SiteDto>>.Ok(sites));
    }
}
