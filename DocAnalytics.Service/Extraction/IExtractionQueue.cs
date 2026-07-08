namespace DocAnalytics.Service.Extraction;

public interface IExtractionQueue
{
    ValueTask EnqueueAsync(Guid fileId, CancellationToken ct = default);
    IAsyncEnumerable<Guid> DequeueAllAsync(CancellationToken ct);
}
