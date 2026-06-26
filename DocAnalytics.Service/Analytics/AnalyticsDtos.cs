using System.ComponentModel.DataAnnotations;

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

// Optional date-range filter for time-series analytics (throughput, error-trend).
public sealed class AnalyticsRangeQuery : IValidatableObject
{
    public DateTime? From { get; set; }   // include data on/after this instant
    public DateTime? To { get; set; }     // include data on/before this instant

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From.HasValue && To.HasValue && From > To)
        {
            yield return new ValidationResult(
                "'from' must be earlier than or equal to 'to'.",
                new[] { nameof(From), nameof(To) });
        }
    }
}

