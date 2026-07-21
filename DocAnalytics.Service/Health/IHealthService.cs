namespace DocAnalytics.Service.Health;

/// <summary>Probes infrastructure health for the health-check endpoint (NFR-4).</summary>
public interface IHealthService
{
    /// <summary>Checks whether the database is reachable via a lightweight query.</summary>
    /// <returns><c>true</c> if the database responded; otherwise <c>false</c>.</returns>
    Task<bool> IsDatabaseReachableAsync();
}
