namespace DocAnalytics.Service.Charts;

public interface IChartService
{
    Task<ChartSeriesDto> GetStatusDistributionAsync(CancellationToken ct = default);
    Task<ChartSeriesDto> GetThroughputAsync(CancellationToken ct = default);
    Task<ChartSeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default);
    Task<ChartSeriesDto> GetErrorTrendAsync(CancellationToken ct = default);


}
