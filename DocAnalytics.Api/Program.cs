using DocAnalytics.Api.Configuration;
using DocAnalytics.Api.Extensions;
using DocAnalytics.Api.Middleware;
using DocAnalytics.Api.Realtime;
using DocAnalytics.Api.Swagger;
using DocAnalytics.Api.Auth;      // JwtSettings
using DocAnalytics.Api.Common;    // ApiResponse<T>
using DocAnalytics.Data;
using DocAnalytics.Data.Seeding;
using DocAnalytics.Service;
using DocAnalytics.Service.Analytics;
using DocAnalytics.Service.Auth;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Service.Health;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Service.Realtime;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(conn) ||
    conn.Contains("SET_VIA_USER_SECRETS", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Default is not configured. Set it via user-secrets or the ConnectionStrings__Default env var.");
}


builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(s => !string.IsNullOrWhiteSpace(s.Key) && s.Key != "SET_VIA_USER_SECRETS",
        "Jwt:Key is missing — set via user-secrets (dev) or env var Jwt__Key (Docker/prod).")
    .Validate(s => !string.IsNullOrEmpty(s.Key) && s.Key.Length >= 32,
        "Jwt:Key must be at least 32 characters (256-bit) for HMAC-SHA256.")
    .ValidateOnStart();

builder.Services.AddSecurityFoundation(builder.Configuration);   // 0.3
builder.Services.AddPersistedDataProtection();                   // 0.4

// Role-based policies (feature: provisioning roles)
builder.Services.AddAuthorization(o =>
{
    // Developer = platform provisioning only
    o.AddPolicy("DeveloperOnly", p => p.RequireClaim("role", "Developer"));
    // Admin = manages users/sites within their own tenant
    o.AddPolicy("AdminOnly", p => p.RequireClaim("role", "Admin"));
    // Business data (dashboards, batches, errors, files…) = tenant users only
    o.AddPolicy("DataAccess", p => p.RequireClaim("role", "Admin", "Viewer"));
});

builder.Services.AddSwaggerWithJwt();                           // Api
builder.Services.AddBatchFeature();
builder.Services.AddHealthFeature();
builder.Services.AddAuthFeature();
builder.Services.AddDashboardFeature();
builder.Services.AddInvoiceFeature();
builder.Services.AddFileDetailsFeature();
builder.Services.AddAnalyticsFeature();
builder.Services.AddErrorListFeature();
builder.Services.AddActivityLogFeature();
builder.Services.AddAlertsFeature();
builder.Services.AddHostedService<DocAnalytics.Api.BackgroundServices.AlertEvaluationBackgroundService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IPipelineNotifier, SignalRPipelineNotifier>();
builder.Services.AddScoped<ISimulationService, SimulationService>();
builder.Services.AddProvisioningFeature();
builder.Services.AddAdminUsersFeature();


builder.Services.AddInvoicePipeline(builder.Configuration);
builder.Services.AddHostedService<DocAnalytics.Api.BackgroundServices.ExtractionWorker>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", httpContext =>
    {
        // Client IP is correct behind nginx because UseForwardedHeaders() runs first (R1).
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });

    options.OnRejected = async (context, token) =>
    {
        var res = context.HttpContext.Response;
        res.StatusCode = StatusCodes.Status429TooManyRequests;
        res.ContentType = "application/json";

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            res.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

        var body = ApiResponse<object>.Fail(
            "RATE_LIMITED", "Too many login attempts. Please try again later.");
        var json = JsonSerializer.Serialize(body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        await res.WriteAsync(json, token);
    };
});

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddValidationBehavior();   // <- Piece B: bad input -> ApiResponse.Fail (400)



var app = builder.Build();
var sec = app.Services.GetRequiredService<IOptions<SecurityOptions>>().Value;

// 1) Trust nginx's X-Forwarded-Proto FIRST — before anything scheme-aware.
if (sec.ForwardedHeaders.Enabled) app.UseForwardedHeaders();

// 2) Global exception envelope — outermost net so it wraps everything below.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 3) HSTS at the public edge (prod only; Dev disables via env + config).
if (!app.Environment.IsDevelopment() && sec.Hsts.Enabled) app.UseHsts();
// app.UseHttpsRedirection();  ❌ leave OFF in-container — nginx terminates TLS (redirect loop otherwise)

// 4) Baseline security headers.
app.UseSecurityHeaders();

// 5) CORS (single, config-driven policy) — before auth.
app.UseCors(CorsOptions.PolicyName);
app.UseRateLimiter();          // ← NEW: throttle before auth work happens

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSiteMiddleware>();

app.MapControllers();
app.MapHub<PipelineHub>("/hubs/pipeline");

app.Run();

