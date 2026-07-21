using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DocAnalytics.Api.Common;


namespace DocAnalytics.Api.Middleware;

/// <summary>Global exception handler: logs unhandled exceptions and returns a generic 500 in the standard <see cref="ApiResponse{T}"/> envelope (no internal detail leaked).</summary>
[ExcludeFromCodeCoverage]
public class ExceptionHandlingMiddleware
{
    // Reuse one options instance; matches the global snake_case JSON policy.
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Creates the middleware with the next delegate and a logger.</summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the pipeline and converts any unhandled exception into a safe 500 JSON response.</summary>
    /// <param name="ctx">The current HTTP context.</param>
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);   // run everything inside the pipeline
        }
        catch (Exception ex)
        {
            // Full detail goes to server logs only — never to the client.
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                ctx.Request.Method, ctx.Request.Path);

            // Can't rewrite a response that already started streaming.
            if (ctx.Response.HasStarted)
                throw;

            ctx.Response.Clear();
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";

            var body = ApiResponse<object>.Fail(
                "internal_error",
                "An unexpected error occurred. Please try again later.");

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts), ctx.RequestAborted);
        }
    }
}
