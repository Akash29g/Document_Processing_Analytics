using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Analytics;

public static class AnalyticsFeatureExtensions
{
    public static IServiceCollection AddAnalyticsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        return services;
    }
}
