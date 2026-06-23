using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Batches;

// 👈 must be this (Program.cs already imports it)

public static class BatchFeatureExtensions
{
    public static IServiceCollection AddBatchFeature(this IServiceCollection services)
    {
        services.AddScoped<IBatchService, BatchService>();
        return services;
    }
}
