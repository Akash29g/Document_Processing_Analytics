namespace DocAnalytics.Service.Analytics;

public interface IAnalyticsService
{
    Task<SeriesDto> GetStatusDistributionAsync(CancellationToken ct = default);
    Task<SeriesDto> GetThroughputAsync(CancellationToken ct = default);
    Task<SeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default);
    Task<SeriesDto> GetErrorTrendAsync(CancellationToken ct = default);


}
