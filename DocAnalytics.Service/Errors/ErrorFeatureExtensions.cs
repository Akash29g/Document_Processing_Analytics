using DocAnalytics.Service.Errors;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

/// <summary>Dependency-injection registration for the Error list/export feature.</summary>
public static class ErrorFeatureExtensions
{
    /// <summary>Registers the error query/export service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddErrorListFeature(this IServiceCollection services)
    {
        services.AddScoped<IErrorService, ErrorService>();
        return services;
    }
}
