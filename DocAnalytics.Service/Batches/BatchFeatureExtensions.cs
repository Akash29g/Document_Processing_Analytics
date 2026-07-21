using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Batches;

/// <summary>Dependency-injection registration for the Batches feature.</summary>
public static class BatchFeatureExtensions
{
    /// <summary>Registers the batch query service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddBatchFeature(this IServiceCollection services)
    {
        services.AddScoped<IBatchService, BatchService>();
        return services;
    }
}
