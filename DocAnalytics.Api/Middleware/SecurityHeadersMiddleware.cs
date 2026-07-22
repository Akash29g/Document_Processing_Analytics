using System.Diagnostics.CodeAnalysis;

namespace DocAnalytics.Api.Middleware;

/// <summary>Adds baseline security response headers to every response (NFR-3 transport hardening).</summary>
[ExcludeFromCodeCoverage]
// Adds baseline security headers to every response (NFR-3 transport hardening).
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>Creates the middleware with the next delegate.</summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    /// <summary>Applies the security headers, then invokes the rest of the pipeline.</summary>
    /// <param name="ctx">The current HTTP context.</param>
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

        // API returns JSON only — lock everything down, block framing entirely.
        ctx.Response.Headers["Content-Security-Policy"] =
            "default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";


        await _next(ctx);
    }
}

/// <summary>Extension helpers for registering <see cref="SecurityHeadersMiddleware"/> in the pipeline.</summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>Adds the security-headers middleware to the application pipeline.</summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same application builder, for chaining.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
