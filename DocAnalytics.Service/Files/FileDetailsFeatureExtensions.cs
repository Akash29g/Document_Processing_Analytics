using DocAnalytics.Service.Files;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

/// <summary>Dependency-injection registration for the File Details feature.</summary>
public static class FileDetailsFeatureExtensions
{
    /// <summary>Registers the file-details service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddFileDetailsFeature(this IServiceCollection services)
    {
        services.AddScoped<IFileDetailsService, FileDetailsService>();
        return services;
    }
}
