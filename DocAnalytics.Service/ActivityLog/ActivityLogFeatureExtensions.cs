using DocAnalytics.Service.ActivityLog;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

/// <summary>Dependency-injection registration for the Activity Log feature.</summary>
public static class ActivityLogFeatureExtensions
{
    /// <summary>Registers the activity log service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddActivityLogFeature(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogService, ActivityLogService>();
        return services;
    }
}
