using DocAnalytics.Service.AdminUsers;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Dependency-injection registration for the Admin Users feature.</summary>
public static class AdminUsersFeatureExtensions
{
    /// <summary>Registers the admin user/site management service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddAdminUsersFeature(this IServiceCollection services)
    {
        services.AddScoped<IAdminUserService, AdminUserService>();
        return services;
    }
}
