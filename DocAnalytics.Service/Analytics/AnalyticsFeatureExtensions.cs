using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Analytics;

/// <summary>Dependency-injection registration for the Analytics feature.</summary>
public static class AnalyticsFeatureExtensions
{
    /// <summary>Registers the analytics/aggregation service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddAnalyticsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        return services;
    }
}
