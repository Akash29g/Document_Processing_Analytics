using DocAnalytics.Api.Extensions;
using DocAnalytics.Api.Middleware;
using DocAnalytics.Data;
using DocAnalytics.Data.Seeding;
using DocAnalytics.Service;
using DocAnalytics.Service.Auth;
using Microsoft.OpenApi;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api
builder.Services.AddSwaggerWithJwt();                           // Api
builder.Services.AddAuthFeature();
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste ONLY your JWT (no 'Bearer ' prefix)."
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });

});


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
