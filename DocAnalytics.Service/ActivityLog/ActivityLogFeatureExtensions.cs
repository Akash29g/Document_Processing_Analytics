using DocAnalytics.Service.ActivityLog;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class ActivityLogFeatureExtensions
{
    public static IServiceCollection AddActivityLogFeature(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogService, ActivityLogService>();
        return services;
    }
}
