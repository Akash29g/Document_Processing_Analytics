using System.Text;
using DocAnalytics.Api.Auth;
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics.CodeAnalysis;



namespace DocAnalytics.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ApiServiceExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
        return services;
    }

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

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
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
