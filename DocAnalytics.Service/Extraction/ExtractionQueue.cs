using System.Threading.Channels;

namespace DocAnalytics.Service.Extraction;

public sealed class ExtractionQueue : IExtractionQueue
{
    private readonly Channel<Guid> _channel =
        Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions { SingleReader = true });

    public ValueTask EnqueueAsync(Guid fileId, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(fileId, ct);

    public IAsyncEnumerable<Guid> DequeueAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
