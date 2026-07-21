using DocAnalytics.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Api.Realtime;

/// <summary>
/// Live pipeline SignalR hub. Clients connect with their JWT then call <see cref="JoinSite"/>
/// to subscribe to their site's group. Broadcasts are scoped to "site:{siteId}" so tenants
/// never receive each other's events (FR-5.3).
/// </summary>
[Authorize]
public sealed class PipelineHub : Hub
{
    private readonly AppDbContext _db;
    public PipelineHub(AppDbContext db) => _db = db;

    /// <summary>Builds the SignalR group name for a given site id.</summary>
    /// <param name="siteId">The site id.</param>
    /// <returns>The group name, e.g. "site:{siteId}".</returns>
    public static string Group(string siteId) => $"site:{siteId}";

    /// <summary>
    /// Subscribes the calling connection to a site's broadcast group after verifying the
    /// authenticated user has access to that site.
    /// </summary>
    /// <param name="siteId">The site id to join.</param>
    /// <exception cref="HubException">Thrown for an invalid site id, an unauthenticated caller, or no site access.</exception>
    public async Task JoinSite(string siteId)
    {
        if (!Guid.TryParse(siteId, out var sid))
            throw new HubException("Invalid site id.");

        // userId comes from the JWT (same claim TenantSiteMiddleware reads).
        var userIdRaw = Context.User?.FindFirst("userId")?.Value;
        if (!Guid.TryParse(userIdRaw, out var uid))
            throw new HubException("Not authenticated.");

        // Enforce site access. UserSiteAccess is NOT tenant-scoped, so the
        // global query filter (which needs CurrentUser) doesn't apply here — safe.
        var hasAccess = await _db.UserSiteAccess
            .AsNoTracking()
            .AnyAsync(x => x.UserId == uid && x.SiteId == sid);

        if (!hasAccess)
            throw new HubException("You do not have access to this site.");

        await Groups.AddToGroupAsync(Context.ConnectionId, Group(siteId));
    }

    /// <summary>Removes the calling connection from a site's broadcast group.</summary>
    /// <param name="siteId">The site id to leave.</param>
    public Task LeaveSite(string siteId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(siteId));
}
