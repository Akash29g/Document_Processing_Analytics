using Microsoft.Extensions.DependencyInjection;
namespace DocAnalytics.Service.Health;

public static class HealthFeatureExtensions
{
    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        return services;
    }
}
