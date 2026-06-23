using DocAnalytics.Data;

namespace DocAnalytics.Service.Health;

public class HealthService : IHealthService
{
    private readonly AppDbContext _db;
    public HealthService(AppDbContext db) => _db = db;

    public Task<bool> IsDatabaseReachableAsync() => _db.Database.CanConnectAsync();
}
