using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Returns the sites the current user may access; populates the global SiteSelector (FR-5.1).
/// </summary>
[ApiController]
[Route("api/v1/sites")]                   // note: /sites, NOT /auth/sites (per DT-2)
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
public class SitesController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates a new <see cref="SitesController"/>.</summary>
    /// <param name="auth">Authentication/authorization service.</param>
    /// <param name="currentUser">The current authenticated user.</param>
    public SitesController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    /// <summary>Returns the list of sites the authenticated user is authorized for (FR-5.1).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user's authorized sites.</returns>
    /// <response code="200">Sites returned.</response>
    [HttpGet]
    public async Task<IActionResult> GetSites(CancellationToken ct)
    {
        var sites = await _auth.GetSitesAsync(_currentUser.UserId, ct);
        return Ok(ApiResponse<IReadOnlyList<SiteDto>>.Ok(sites));
    }
}
