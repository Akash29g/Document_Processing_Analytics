using System.Diagnostics.CodeAnalysis;
using DocAnalytics.Service.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace DocAnalytics.Api.Realtime;

/// <summary>
/// SignalR-backed <see cref="IPipelineNotifier"/> implementation. Lives in the Api project
/// because it depends on <see cref="IHubContext{THub}"/>; pushes file-state changes to the
/// subscribers of the relevant site's group.
/// </summary>
[ExcludeFromCodeCoverage]
// Concrete implementation of the Service-layer abstraction.
// Lives in the Api project because IHubContext / SignalR are here.
public sealed class SignalRPipelineNotifier : IPipelineNotifier
{
    private readonly IHubContext<PipelineHub> _hub;
    public SignalRPipelineNotifier(IHubContext<PipelineHub> hub) => _hub = hub;

    /// <inheritdoc />
    public Task NotifyFileStateChangedAsync(
        Guid siteId, FileStateChangedNotification notification, CancellationToken ct = default)
        => _hub.Clients
            .Group(PipelineHub.Group(siteId.ToString()))     // only that site's subscribers
            .SendAsync("FileStateChanged", notification, ct); // client listens for "FileStateChanged"
}
