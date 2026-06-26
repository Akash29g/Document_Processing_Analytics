namespace DocAnalytics.Service.Charts;

public sealed class ChartSeriesDto
{
    public List<ChartPointDto> Points { get; set; } = new();
}

public sealed class ChartPointDto
{
    public string Label { get; set; } = null!;   // "Completed", "2023-10-21", "ERR_OCR_40"
    public long Value { get; set; }               // the count
}
