namespace DocAnalytics.Service.Realtime;

/// <summary>Development-only helper that simulates a pipeline state change to demo real-time updates.</summary>
public interface ISimulationService
{
    /// <summary>Flips one file's state, updates counters, writes an audit row, and broadcasts the change.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The state-change notification, or <c>null</c> if there are no files for this site.</returns>
    // Flips one file's state, updates counters, writes an audit row, broadcasts.
    // Returns the change (or null if there are no files for this site).
    Task<FileStateChangedNotification?> SimulateStateChangeAsync(CancellationToken ct = default);
}
