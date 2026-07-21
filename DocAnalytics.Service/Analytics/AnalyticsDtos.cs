using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Analytics;

/// <summary>A chart series — a list of labelled data points.</summary>
public sealed class SeriesDto
{
    /// <summary>The data points in the series.</summary>
    public List<SeriesPointDto> Points { get; set; } = new();
}

/// <summary>A single labelled value in a chart series.</summary>
public sealed class SeriesPointDto
{
    /// <summary>The point label, e.g. "Completed", "2023-10-21", or "ERR_OCR_40".</summary>
    public string Label { get; set; } = null!;   // "Completed", "2023-10-21", "ERR_OCR_40"
    /// <summary>The point's count/value.</summary>
    public long Value { get; set; }               // the count
}

/// <summary>Per-step processing-time percentiles (S-5); serialized to snake_case.</summary>
// Per-step processing-time percentiles (S-5). snake_case'd globally.
public sealed class StepPercentileDto
{
    /// <summary>The pipeline step name.</summary>
    public string Step { get; set; } = null!;   // → "step"
    /// <summary>Number of completed steps counted for this percentile.</summary>
    public int SampleCount { get; set; }         // → "sample_count"  (completed steps counted)
    /// <summary>50th-percentile duration in seconds.</summary>
    public double P50Seconds { get; set; }       // → "p50_seconds"
    /// <summary>90th-percentile duration in seconds.</summary>
    public double P90Seconds { get; set; }       // → "p90_seconds"
    /// <summary>99th-percentile duration in seconds.</summary>
    public double P99Seconds { get; set; }       // → "p99_seconds"
}

/// <summary>Optional date-range filter for time-series analytics (throughput, error-trend).</summary>
// Optional date-range filter for time-series analytics (throughput, error-trend).
public sealed class AnalyticsRangeQuery : IValidatableObject
{
    /// <summary>Include data on/after this instant.</summary>
    public DateTime? From { get; set; }   // include data on/after this instant
    /// <summary>Include data on/before this instant.</summary>
    public DateTime? To { get; set; }     // include data on/before this instant

    /// <inheritdoc />
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
