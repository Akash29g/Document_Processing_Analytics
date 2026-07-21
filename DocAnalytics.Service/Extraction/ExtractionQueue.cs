using System.Threading.Channels;

namespace DocAnalytics.Service.Extraction;

/// <summary>Default <see cref="IExtractionQueue"/> implementation backed by an unbounded in-process channel.</summary>
public sealed class ExtractionQueue : IExtractionQueue
{
    private readonly Channel<Guid> _channel =
        Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions { SingleReader = true });

    /// <inheritdoc />
    public ValueTask EnqueueAsync(Guid fileId, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(fileId, ct);

    /// <inheritdoc />
    public IAsyncEnumerable<Guid> DequeueAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
