using System.Text.Json;
using System.Threading.RateLimiting;
using DocAnalytics.Api.Common;   // ApiResponse<T>

namespace DocAnalytics.Api.Configuration;

/// <summary>Registers the ASP.NET Core rate limiter with login/reads/export policies (Round 5) and a standard 429 envelope.</summary>
public static class RateLimitingExtensions
{
    /// <summary>Policy name for login endpoints (partitioned by client IP).</summary>
    public const string LoginPolicy = "login";
    /// <summary>Policy name for read endpoints (partitioned by authenticated user).</summary>
    public const string ReadsPolicy = "reads";
    /// <summary>Policy name for export endpoints (tight, partitioned by user).</summary>
    public const string ExportPolicy = "export";
    /// <summary>Policy name for 2FA verification endpoints (partitioned by user, else IP).</summary>
    public const string MfaPolicy = "mfa";


    /// <summary>Configures the rate limiter, its policies, and the 429 rejection response.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The application configuration (reads the "RateLimiting" section).</param>
    /// <returns>The same service collection, for chaining.</returns>
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

            // mfa: 2FA code attempts — per authenticated user where available, else per IP
            // (UserKey already falls back to IP, exactly what "partitioned by user/IP" means here).
            options.AddPolicy(MfaPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(UserKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = opts.Mfa.PermitLimit,
                    Window = TimeSpan.FromSeconds(opts.Mfa.WindowSeconds),
                    QueueLimit = opts.Mfa.QueueLimit,
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
