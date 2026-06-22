using DocAnalytics.Service.Abstractions;
using DocAnalytics.Service.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        // Dev A/B add their feature services here later:
        // services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBatchService, BatchService>(); 
        return services;
    }
}
