using DocAnalytics.Service.Realtime;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;


namespace DocAnalytics.Api.Realtime;

[ExcludeFromCodeCoverage]
// Concrete implementation of the Service-layer abstraction.
// Lives in the Api project because IHubContext / SignalR are here.
public sealed class SignalRPipelineNotifier : IPipelineNotifier
{
    private readonly IHubContext<PipelineHub> _hub;
    public SignalRPipelineNotifier(IHubContext<PipelineHub> hub) => _hub = hub;

    public Task NotifyFileStateChangedAsync(
        Guid siteId, FileStateChangedNotification notification, CancellationToken ct = default)
        => _hub.Clients
            .Group(PipelineHub.Group(siteId.ToString()))     // only that site's subscribers
            .SendAsync("FileStateChanged", notification, ct); // client listens for "FileStateChanged"
}
