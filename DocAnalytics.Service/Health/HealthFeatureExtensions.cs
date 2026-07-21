using Microsoft.Extensions.DependencyInjection;
namespace DocAnalytics.Service.Health;

/// <summary>Dependency-injection registration for the Health feature.</summary>
public static class HealthFeatureExtensions
{
    /// <summary>Registers the health-check service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        return services;
    }
}
