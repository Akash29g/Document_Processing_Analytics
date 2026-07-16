using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Auth;

public static class AuthFeatureExtensions
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ILoginLockoutService, LoginLockoutService>();   // ← the only new line
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();   // NEW (R4)
        return services;
    }
}
