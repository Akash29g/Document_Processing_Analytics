namespace DocAnalytics.Service.Extraction;

/// <summary>In-process queue that hands uploaded file ids to the background extraction worker.</summary>
public interface IExtractionQueue
{
    /// <summary>Enqueues a file id for asynchronous extraction.</summary>
    /// <param name="fileId">The file to process.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask EnqueueAsync(Guid fileId, CancellationToken ct = default);

    /// <summary>Streams queued file ids to the consumer until cancellation.</summary>
    /// <param name="ct">Cancellation token that stops the stream.</param>
    /// <returns>An async stream of file ids to process.</returns>
    IAsyncEnumerable<Guid> DequeueAllAsync(CancellationToken ct);
}
