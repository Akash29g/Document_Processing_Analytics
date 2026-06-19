using DocAnalytics.Data;
using DocAnalytics.Service.Abstractions;

namespace DocAnalytics.Service.Implementations;

public class HealthService : IHealthService
{
    private readonly AppDbContext _db;
    public HealthService(AppDbContext db) => _db = db;

    public Task<bool> IsDatabaseReachableAsync() => _db.Database.CanConnectAsync();
}
