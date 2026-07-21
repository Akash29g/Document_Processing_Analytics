using System.Diagnostics.CodeAnalysis;
using System.Text;
using DocAnalytics.Api.Auth;
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;



namespace DocAnalytics.Api.Extensions;

/// <summary>DI wiring for API concerns: current-user accessor, JWT authentication, and Swagger with JWT + site-header support.</summary>
[ExcludeFromCodeCoverage]
public static class ApiServiceExtensions
{
    /// <summary>Registers the request-scoped <see cref="CurrentUser"/> and exposes it as <see cref="ICurrentUser"/>.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
        return services;
    }

    /// <summary>Configures JWT bearer authentication (including SignalR query-string tokens for /hubs) and authorization.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="cfg">The application configuration (reads the "Jwt" section).</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        var settings = cfg.GetSection("Jwt").Get<JwtSettings>()!;
        services.Configure<JwtSettings>(cfg.GetSection("Jwt"));

        services.AddAuthentication("Bearer").AddJwtBearer("Bearer", o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                RoleClaimType = "role"
            };

            o.MapInboundClaims = false;   // keep JWT claim names as-is ("role" stays "role")

            // ── S-1: allow SignalR to authenticate the WebSocket via ?access_token= ──
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;   // use the query-string token for hub requests
                    }
                    return Task.CompletedTask;
                }
            };
        });
        services.AddAuthorization();
        return services;
    }

    /// <summary>Configures Swagger/OpenAPI, including XML comments from all DocAnalytics assemblies and Bearer + X-Site-Id security schemes.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {

            // Include XML docs from Api + Service + Domain so summaries/params render in Swagger UI
            foreach (var xml in Directory.GetFiles(AppContext.BaseDirectory, "DocAnalytics.*.xml"))
            {
                options.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste ONLY your JWT (no 'Bearer ' prefix)."
            });

            options.AddSecurityDefinition("SiteId", new OpenApiSecurityScheme
            {
                Name = "X-Site-Id",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "Paste your site_id GUID once — applied to every request (tenant/site isolation)."
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() },
            { new OpenApiSecuritySchemeReference("SiteId", doc), new List<string>() }
        });
        });
        return services;
    }


}
