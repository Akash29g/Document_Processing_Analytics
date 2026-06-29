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
using Microsoft.OpenApi;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api
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

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddValidationBehavior();   // <- Piece B: bad input -> ApiResponse.Fail (400)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseMiddleware<ExceptionHandlingMiddleware>();   // outermost net — catches everything below
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSiteMiddleware>();
app.MapControllers();

app.Run();
