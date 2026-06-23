namespace DocAnalytics.Service.Health;

public interface IHealthService
{
    Task<bool> IsDatabaseReachableAsync();
}
