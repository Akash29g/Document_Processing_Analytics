namespace DocAnalytics.Service.Realtime;

/// <summary>Broadcast abstraction so the Service layer can push updates without depending on SignalR.</summary>
// Abstraction so the Service layer can broadcast WITHOUT depending on SignalR.
// The concrete SignalR implementation lives in the Api layer.
public interface IPipelineNotifier
{
    /// <summary>Pushes a file state-change notification to the given site's clients.</summary>
    /// <param name="siteId">The site whose clients should receive the update.</param>
    /// <param name="notification">The change payload.</param>
    /// <param name="ct">Cancellation token.</param>
    Task NotifyFileStateChangedAsync(Guid siteId, FileStateChangedNotification notification, CancellationToken ct = default);
}

/// <summary>Real-time payload pushed to clients when a file's state changes (serialized to snake_case).</summary>
/// <param name="FileId">The file whose state changed.</param>
/// <param name="FileName">The file name.</param>
/// <param name="OldState">The previous state, if known.</param>
/// <param name="NewState">The new state.</param>
/// <param name="Step">The pipeline step at which the change occurred.</param>
/// <param name="At">The change timestamp (UTC).</param>
// The payload pushed to clients. Property names serialize to snake_case
// via the global JsonNamingPolicy.SnakeCaseLower (same as your REST DTOs).
public sealed record FileStateChangedNotification(
    Guid FileId,
    string FileName,
    string? OldState,
    string NewState,
    string Step,
    DateTime At);
