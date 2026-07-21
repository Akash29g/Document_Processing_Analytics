using DocAnalytics.Service.Provisioning;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

/// <summary>Dependency-injection registration for the Provisioning feature.</summary>
public static class ProvisioningFeatureExtensions
{
    /// <summary>Registers the provisioning service and credential generator in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddProvisioningFeature(this IServiceCollection services)
    {
        services.AddScoped<IProvisioningService, ProvisioningService>();
        services.AddSingleton<ICredentialGenerator, CredentialGenerator>();
        return services;
    }
}
