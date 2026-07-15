using System.Text.Json;
using DocAnalytics.Api.Common;
using System.Diagnostics.CodeAnalysis;


namespace DocAnalytics.Api.Middleware;

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

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

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
