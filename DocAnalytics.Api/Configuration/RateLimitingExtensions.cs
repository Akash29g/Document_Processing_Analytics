using System.Text.Json;
using System.Threading.RateLimiting;
using DocAnalytics.Api.Common;   // ApiResponse<T>

namespace DocAnalytics.Api.Configuration;

public static class RateLimitingExtensions
{
    public const string LoginPolicy = "login";
    public const string ReadsPolicy = "reads";
    public const string ExportPolicy = "export";

    public static IServiceCollection AddRateLimitingFeature(
        this IServiceCollection services, IConfiguration config)
    {
        var opts = config.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>()
                   ?? new RateLimitOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // login: by client IP (R1: correct behind nginx via UseForwardedHeaders)
            options.AddPolicy(LoginPolicy, httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = opts.Login.PermitLimit,
                    Window = TimeSpan.FromSeconds(opts.Login.WindowSeconds),
                    QueueLimit = opts.Login.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // reads: per authenticated user (IP fallback)
            options.AddPolicy(ReadsPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(UserKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = opts.Reads.PermitLimit,
                    Window = TimeSpan.FromSeconds(opts.Reads.WindowSeconds),
                    QueueLimit = opts.Reads.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));

            // export: tight, per user
            options.AddPolicy(ExportPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(UserKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = opts.Export.PermitLimit,
                    Window = TimeSpan.FromSeconds(opts.Export.WindowSeconds),
                    QueueLimit = opts.Export.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));

            // 429 body in the standard ApiResponse envelope + Retry-After (your R1 code, generic message)
            options.OnRejected = async (context, token) =>
            {
                var res = context.HttpContext.Response;
                res.StatusCode = StatusCodes.Status429TooManyRequests;
                res.ContentType = "application/json";

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    res.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

                var body = ApiResponse<object>.Fail(
                    "RATE_LIMITED", "Too many requests. Please try again later.");
                var json = JsonSerializer.Serialize(body,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

                await res.WriteAsync(json, token);
            };
        });

        return services;
    }

    private static string ClientIp(HttpContext ctx) =>
        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static string UserKey(HttpContext ctx) =>
        ctx.User.FindFirst("userId")?.Value ?? ClientIp(ctx);
}
