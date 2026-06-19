using System.Text;
using DocAnalytics.Api.Auth;
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using Microsoft.IdentityModel.Tokens;

namespace DocAnalytics.Api.Extensions;

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
        services.AddSingleton<JwtTokenService>();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
            };
        });
        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   // plain for now; JWT "Authorize" button added with the auth slice
        return services;
    }
}
