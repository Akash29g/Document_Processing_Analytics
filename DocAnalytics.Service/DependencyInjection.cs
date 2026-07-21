using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service;

/// <summary>Aggregate application-service registration entry point (feature services are wired via their own AddXxxFeature extensions).</summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    /// <summary>Registers core application services in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddScoped<IHealthService, HealthService>();
        // Dev A/B add their feature services here later:
        // services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<IBatchService, BatchService>(); 
        return services;
    }
}
