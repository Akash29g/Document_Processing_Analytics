using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Dashboard;

public static class DashboardFeatureExtensions
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
