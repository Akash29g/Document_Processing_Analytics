using DocAnalytics.Api.Extensions;
using DocAnalytics.Api.Middleware;
using DocAnalytics.Api.Swagger;
using DocAnalytics.Data;
using DocAnalytics.Data.Seeding;
using DocAnalytics.Service;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Service.Auth;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Health;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Service.Analytics;
using DocAnalytics.Api.Realtime;
using DocAnalytics.Service.Realtime;
using Microsoft.OpenApi;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api

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


builder.Services.AddInvoicePipeline(builder.Configuration);
builder.Services.AddHostedService<DocAnalytics.Api.BackgroundServices.ExtractionWorker>();



builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddValidationBehavior();   // <- Piece B: bad input -> ApiResponse.Fail (400)

builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.WithOrigins("http://localhost:4200")
     .AllowAnyHeader()     // allows Authorization + X-Site-Id
     .AllowAnyMethod()));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseMiddleware<ExceptionHandlingMiddleware>();   // outermost net — catches everything below
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSiteMiddleware>();
app.MapControllers();
app.MapHub<PipelineHub>("/hubs/pipeline");   // ← S-1: SignalR endpoint

app.Run();
