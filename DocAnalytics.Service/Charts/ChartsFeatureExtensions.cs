using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Charts;

public static class ChartsFeatureExtensions
{
    public static IServiceCollection AddChartsFeature(this IServiceCollection services)
    {
        services.AddScoped<IChartService, ChartService>();
        return services;
    }
}
