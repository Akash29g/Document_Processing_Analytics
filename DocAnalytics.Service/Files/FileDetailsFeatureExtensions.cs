using DocAnalytics.Service.Files;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class FileDetailsFeatureExtensions
{
    public static IServiceCollection AddFileDetailsFeature(this IServiceCollection services)
    {
        services.AddScoped<IFileDetailsService, FileDetailsService>();
        return services;
    }
}
