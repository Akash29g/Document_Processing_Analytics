using DocAnalytics.Data;

namespace DocAnalytics.Service.Health;

/// <summary>Default <see cref="IHealthService"/> implementation: verifies database connectivity.</summary>
public class HealthService : IHealthService
{
    private readonly AppDbContext _db;
    public HealthService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public Task<bool> IsDatabaseReachableAsync() => _db.Database.CanConnectAsync();
}
