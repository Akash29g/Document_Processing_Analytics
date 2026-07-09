using DocAnalytics.Service.Provisioning;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class ProvisioningFeatureExtensions
{
    public static IServiceCollection AddProvisioningFeature(this IServiceCollection services)
    {
        services.AddScoped<IProvisioningService, ProvisioningService>();
        services.AddSingleton<ICredentialGenerator, CredentialGenerator>();
        return services;
    }
}
