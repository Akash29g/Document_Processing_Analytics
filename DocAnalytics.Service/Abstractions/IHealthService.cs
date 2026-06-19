namespace DocAnalytics.Service.Abstractions;

public interface IHealthService
{
    Task<bool> IsDatabaseReachableAsync();
}
