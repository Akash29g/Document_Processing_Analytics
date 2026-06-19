using DocAnalytics.Api.Extensions;
using DocAnalytics.Api.Middleware;
using DocAnalytics.Data;
using DocAnalytics.Data.Seeding;
using DocAnalytics.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api
builder.Services.AddSwaggerWithJwt();                           // Api

var app = builder.Build();

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

app.Run();
