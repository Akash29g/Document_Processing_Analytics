namespace DocAnalytics.Service.Realtime;

public interface ISimulationService
{
    // Flips one file's state, updates counters, writes an audit row, broadcasts.
    // Returns the change (or null if there are no files for this site).
    Task<FileStateChangedNotification?> SimulateStateChangeAsync(CancellationToken ct = default);
}
