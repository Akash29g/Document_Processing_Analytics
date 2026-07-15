namespace DocAnalytics.Service.Realtime;

// Abstraction so the Service layer can broadcast WITHOUT depending on SignalR.
// The concrete SignalR implementation lives in the Api layer.
public interface IPipelineNotifier
{
    Task NotifyFileStateChangedAsync(Guid siteId, FileStateChangedNotification notification, CancellationToken ct = default);
}

// The payload pushed to clients. Property names serialize to snake_case
// via the global JsonNamingPolicy.SnakeCaseLower (same as your REST DTOs).
public sealed record FileStateChangedNotification(
    Guid FileId,
    string FileName,
    string? OldState,
    string NewState,
    string Step,
    DateTime At);
