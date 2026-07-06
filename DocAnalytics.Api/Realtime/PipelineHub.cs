using System.Security.Claims;
using DocAnalytics.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Api.Realtime;

// Live pipeline hub. Clients connect with their JWT, then call JoinSite(siteId)
// to subscribe to their site's group. Broadcasts are scoped to "site:{siteId}"
// so tenants never receive each other's events (FR-5.3).
[Authorize]
public sealed class PipelineHub : Hub
{
    private readonly AppDbContext _db;
    public PipelineHub(AppDbContext db) => _db = db;

    public static string Group(string siteId) => $"site:{siteId}";

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

    public Task LeaveSite(string siteId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(siteId));
}
