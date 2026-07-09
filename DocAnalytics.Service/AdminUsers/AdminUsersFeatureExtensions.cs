using DocAnalytics.Service.AdminUsers;

namespace Microsoft.Extensions.DependencyInjection;

public static class AdminUsersFeatureExtensions
{
    public static IServiceCollection AddAdminUsersFeature(this IServiceCollection services)
    {
        services.AddScoped<IAdminUserService, AdminUserService>();
        return services;
    }
}
