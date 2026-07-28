using DocAnalytics.Service.ErrorCatalog;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>DI registration for the Error Catalog management feature.</summary>
public static class ErrorCatalogFeatureExtensions
{
    public static IServiceCollection AddErrorCatalogFeature(this IServiceCollection services)
    {
        services.AddScoped<IErrorCatalogService, ErrorCatalogService>();
        return services;
    }
}
