using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Dashboard;

/// <summary>Dependency-injection registration for the Dashboard feature.</summary>
public static class DashboardFeatureExtensions
{
    /// <summary>Registers the dashboard service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
