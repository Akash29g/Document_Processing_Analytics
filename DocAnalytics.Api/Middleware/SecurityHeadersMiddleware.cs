namespace DocAnalytics.Api.Middleware;

// Adds baseline security headers to every response (NFR-3 transport hardening).
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        var h = ctx.Response.Headers;

        // Stop the browser MIME-sniffing responses into something executable.
        h["X-Content-Type-Options"] = "nosniff";
        // Disallow this API's responses being embedded in a frame (clickjacking).
        h["X-Frame-Options"] = "DENY";
        // Don't leak the requested URL to other origins.
        h["Referrer-Policy"] = "no-referrer";
        // Lock down legacy cross-domain policy files.
        h["X-Permitted-Cross-Domain-Policies"] = "none";
        // Trim server fingerprinting where we can.
        h.Remove("X-Powered-By");

        await _next(ctx);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
