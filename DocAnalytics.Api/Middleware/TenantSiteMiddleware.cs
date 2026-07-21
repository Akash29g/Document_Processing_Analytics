using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using DocAnalytics.Api.Common;          // ApiResponse<T>
using DocAnalytics.Data;                // AppDbContext
using Microsoft.EntityFrameworkCore;    // AnyAsync


namespace DocAnalytics.Api.Middleware;

/// <summary>
/// Populates the request's tenant/site context on <see cref="CurrentUser"/> from JWT claims and the site header/query.
/// Hard-blocks the Developer role from business-data routes and enforces per-site access (FR-5.3) before the request proceeds.
/// </summary>
[ExcludeFromCodeCoverage]
public class TenantSiteMiddleware
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly RequestDelegate _next;

    /// <summary>Creates the middleware with the next delegate.</summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    public TenantSiteMiddleware(RequestDelegate next) => _next = next;

    /// <summary>Resolves and validates tenant/site context for the request, then invokes the rest of the pipeline.</summary>
    /// <param name="ctx">The current HTTP context.</param>
    /// <param name="currentUser">The request-scoped current-user accessor to populate.</param>
    /// <param name="db">The database context, used to verify site access.</param>
    public async Task Invoke(HttpContext ctx, CurrentUser currentUser, AppDbContext db)
    {
        // Developer = provisioning only — hard-block all data routes at the API level,
        // even if a controller attribute is ever forgotten.
        var roleClaim = ctx.User.FindFirstValue("role");
        if (roleClaim == "Developer")
        {
            var path = ctx.Request.Path.Value ?? string.Empty;
            var allowed = path.StartsWith("/api/v1/auth", StringComparison.OrdinalIgnoreCase)
                       || path.StartsWith("/api/v1/provisioning", StringComparison.OrdinalIgnoreCase)
                       || path.StartsWith("/api/v1/health", StringComparison.OrdinalIgnoreCase);
            if (!allowed)
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                ctx.Response.ContentType = "application/json";
                var forbidden = ApiResponse<object>.Fail(
                    "FORBIDDEN_ROLE", "Developer role has no access to business data.");
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(forbidden, JsonOpts));
                return;
            }
            // Developer has no tenant/site — set identity only, skip tenant context
            var devUserIdRaw = ctx.User.FindFirstValue("userId");
            if (Guid.TryParse(devUserIdRaw, out var devUid))
                currentUser.Set(devUid, Guid.Empty, Guid.Empty, "Developer");

            await _next(ctx);
            return;
        }

        if (ctx.User.Identity?.IsAuthenticated == true)
        {
            var userId = ctx.User.FindFirstValue("userId");
            var tenantId = ctx.User.FindFirstValue("tenantId");
            var role = ctx.User.FindFirstValue("role") ?? "Viewer";
            var siteIdRaw = ctx.Request.Query["site_id"].FirstOrDefault()
                            ?? ctx.Request.Headers["X-Site-Id"].FirstOrDefault();

            if (Guid.TryParse(userId, out var uid) && Guid.TryParse(tenantId, out var tid))
            {
                Guid.TryParse(siteIdRaw, out var sid);

                // NEW: if a site is supplied, the user must be granted access to it (FR-5.3)
                if (sid != Guid.Empty)
                {
                    var hasAccess = await db.UserSiteAccess
                        .AsNoTracking()
                        .AnyAsync(x => x.UserId == uid && x.SiteId == sid);

                    if (!hasAccess)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        ctx.Response.ContentType = "application/json";
                        var body = ApiResponse<object>.Fail(
                            "SITE_FORBIDDEN",
                            "You do not have access to the requested site.");
                        await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts));
                        return;
                    }
                }

                currentUser.Set(uid, tid, sid, role);
            }
        }

        await _next(ctx);
    }
}
