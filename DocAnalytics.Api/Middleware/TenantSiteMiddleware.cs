using System.Security.Claims;
using DocAnalytics.Api.Common;

namespace DocAnalytics.Api.Middleware;

public class TenantSiteMiddleware
{
    private readonly RequestDelegate _next;
    public TenantSiteMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx, CurrentUser currentUser)
    {
        if (ctx.User.Identity?.IsAuthenticated == true)
        {
            var userId = ctx.User.FindFirstValue("userId");      // 👈 was NameIdentifier/"sub"
            var tenantId = ctx.User.FindFirstValue("tenantId");    // 👈 was "tenant_id"
            var role = ctx.User.FindFirstValue("role") ?? "Viewer";   // 👈 was ClaimTypes.Role
            var siteIdRaw = ctx.Request.Query["site_id"].FirstOrDefault()
                            ?? ctx.Request.Headers["X-Site-Id"].FirstOrDefault();

            if (Guid.TryParse(userId, out var uid) && Guid.TryParse(tenantId, out var tid))
            {
                Guid.TryParse(siteIdRaw, out var sid);
                currentUser.Set(uid, tid, sid, role);
            }
        }
        await _next(ctx);
    }
}
