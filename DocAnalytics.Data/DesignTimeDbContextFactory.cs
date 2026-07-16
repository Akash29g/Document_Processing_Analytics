using DocAnalytics.Domain.Common;          // ICurrentUser
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocAnalytics.Data;

// Used ONLY by `dotnet ef` / the migration bundle. Lets EF build AppDbContext
// without starting the API host (which needs JWT/AWS config that isn't present
// in the migration container). DDL doesn't need a real tenant, so we stub ICurrentUser.
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conn =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new AppDbContext(options, new MigrationCurrentUser());
    }

    // No-op current user — migrations run DDL, not tenant-filtered queries.
    private sealed class MigrationCurrentUser : ICurrentUser
    {
        public Guid UserId => Guid.Empty;
        public Guid TenantId => Guid.Empty;
        public Guid SiteId => Guid.Empty;
        public string Role => "Developer";
    }
}
