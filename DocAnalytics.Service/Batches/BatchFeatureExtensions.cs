using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Batches;

public static class BatchFeatureExtensions
{
    public static IServiceCollection AddBatchFeature(this IServiceCollection services)
    {
        services.AddScoped<IBatchService, BatchService>();
        return services;
    }
}
