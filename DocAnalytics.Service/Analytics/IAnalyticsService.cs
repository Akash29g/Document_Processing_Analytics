namespace DocAnalytics.Service.Analytics;

/// <summary>Aggregation queries powering the dashboard and error-analysis charts (FR-1, FR-3, S-5).</summary>
public interface IAnalyticsService
{
    /// <summary>Returns the current file status distribution for the pie/bar chart (FR-1.3).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A labelled series of status counts.</returns>
    Task<SeriesDto> GetStatusDistributionAsync(CancellationToken ct = default);

    /// <summary>Returns processing throughput over an optional date range (FR-1.2).</summary>
    /// <param name="from">Optional inclusive start.</param>
    /// <param name="to">Optional inclusive end.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A time-bucketed series of completed-file counts.</returns>
    Task<SeriesDto> GetThroughputAsync(DateTime? from, DateTime? to, CancellationToken ct = default);

    /// <summary>Returns the top-N most frequent errors with counts (FR-3.1).</summary>
    /// <param name="topN">Number of top errors to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A labelled series of error codes and counts.</returns>
    Task<SeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default);

    /// <summary>Returns the number of failures per bucket over an optional date range (FR-3.2).</summary>
    /// <param name="from">Optional inclusive start.</param>
    /// <param name="to">Optional inclusive end.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A time-bucketed series of failure counts.</returns>
    Task<SeriesDto> GetErrorTrendAsync(DateTime? from, DateTime? to, CancellationToken ct = default);

    /// <summary>Returns P50/P90/P99 processing-time percentiles per pipeline step (S-5).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of per-step percentile rows.</returns>
    Task<List<StepPercentileDto>> GetStepPercentilesAsync(CancellationToken ct = default);



}
