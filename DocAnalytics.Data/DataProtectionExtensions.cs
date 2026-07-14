using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Data;

public static class DataProtectionExtensions
{
    /// <summary>
    /// Persists the DataProtection key ring to Postgres so keys survive
    /// container restarts (prevents refresh-token / cookie invalidation).
    /// </summary>
    public static IServiceCollection AddPersistedDataProtection(this IServiceCollection services)
    {
        services.AddDataProtection()
                .SetApplicationName("DocAnalytics")          // stable across all instances
                .PersistKeysToDbContext<AppDbContext>();
        // TODO (R5 hardening): .ProtectKeysWithCertificate(...) or AWS KMS for keys-at-rest.
        return services;
    }
}
