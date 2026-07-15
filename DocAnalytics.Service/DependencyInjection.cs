using DocAnalytics.Service.Health;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DocAnalytics.Service;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddScoped<IHealthService, HealthService>();
        // Dev A/B add their feature services here later:
        // services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<IBatchService, BatchService>(); 
        return services;
    }
}
