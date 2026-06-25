using DocAnalytics.Service.Errors;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class ErrorFeatureExtensions
{
    public static IServiceCollection AddErrorListFeature(this IServiceCollection services)
    {
        services.AddScoped<IErrorService, ErrorService>();
        return services;
    }
}
