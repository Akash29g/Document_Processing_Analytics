using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Auth;

/// <summary>Dependency-injection registration for the Auth feature (auth, JWT, lockout, refresh tokens).</summary>
public static class AuthFeatureExtensions
{
    /// <summary>Registers the authentication services in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddHttpClient<IPasswordPolicy, PasswordPolicy>(c => c.Timeout = TimeSpan.FromSeconds(3));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ILoginLockoutService, LoginLockoutService>();   // ← the only new line
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();   // NEW (R4)
        services.AddScoped<IPasswordResetService, PasswordResetService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();         // NEW (Account Security)
        return services;

    }
}
