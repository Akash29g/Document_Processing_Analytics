namespace DocAnalytics.Service.Analytics;

public sealed class SeriesDto
{
    public List<SeriesPointDto> Points { get; set; } = new();
}

public sealed class SeriesPointDto
{
    public string Label { get; set; } = null!;   // "Completed", "2023-10-21", "ERR_OCR_40"
    public long Value { get; set; }               // the count
}
