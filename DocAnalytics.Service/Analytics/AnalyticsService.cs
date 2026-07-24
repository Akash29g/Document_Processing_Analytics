using DocAnalytics.Data;
using DocAnalytics.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Analytics;



/// <summary>Default <see cref="IAnalyticsService"/> implementation; EF Core aggregations for dashboard/error charts.</summary>
public sealed class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;
    public AnalyticsService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<SeriesDto> GetStatusDistributionAsync(CancellationToken ct = default)
    {
        var points = await _db.Files
            .AsNoTracking()
            .GroupBy(f => f.Status)                    // bucket files by their Status
            .Select(g => new SeriesPointDto
            {
                Label = g.Key,                         // the status value, e.g. "Completed"
                Value = g.LongCount()                  // COUNT(*) for that status
            })
            .OrderByDescending(p => p.Value)           // biggest slice first
            .ThenBy(p => p.Label)                      // tiebreaker → deterministic order
            .ToListAsync(ct);

        return new SeriesDto { Points = points };
    }

    /// <inheritdoc />
    public async Task<SeriesDto> GetThroughputAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _db.Files
            .AsNoTracking()
            .Where(f => f.Status == "Completed");                 // FR-1.2: completed only

        if (from.HasValue)
        {
            var fromUtc = from.Value.AsUtc();
            q = q.Where(f => f.LastUpdatedAt >= fromUtc);   // optional lower bound (UTC-normalised)
        }
        if (to.HasValue)
        {
            var toUtc = to.Value.AsUtc();
            q = q.Where(f => f.LastUpdatedAt <= toUtc);     // optional upper bound (UTC-normalised)
        }

        var raw = await q
            .GroupBy(f => f.LastUpdatedAt.Date)                   // bucket by completion day
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Day.ToString("yyyy-MM-dd"), Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }


    /// <inheritdoc />
    public async Task<SeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default)
    {
        // Light guard so a silly topN can't break the chart (full validation = Round 5).
        if (topN < 1) topN = 5;
        if (topN > 20) topN = 20;

        var raw = await _db.Files                    // ① ANCHOR on the tenant-scoped table
            .AsNoTracking()
            .SelectMany(f => f.Steps)                // ② navigate OUT to non-scoped FileStepHistory
            .Where(s => s.ErrorCode != null)         // ③ only steps that actually errored
            .GroupBy(s => s.ErrorCode!)              // ④ bucket by error code
            .Select(g => new { Code = g.Key, Count = g.LongCount() })
            .OrderByDescending(x => x.Count)         // ⑤ most frequent first
            .ThenBy(x => x.Code)                     // ⑥ deterministic tiebreaker
            .Take(topN)                              // ⑦ TOP N → SQL LIMIT
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Code, Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }

    /// <inheritdoc />
    public async Task<SeriesDto> GetErrorTrendAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _db.Files
            .AsNoTracking()
            .SelectMany(f => f.Steps)
            .Where(s => s.ErrorCode != null && s.StartedAt != null);   // errored AND timestamped

        if (from.HasValue)
        {
            var fromUtc = from.Value.AsUtc();
            q = q.Where(s => s.StartedAt >= fromUtc);
        }
        if (to.HasValue)
        {
            var toUtc = to.Value.AsUtc();
            q = q.Where(s => s.StartedAt <= toUtc);
        }

        var raw = await q
            .GroupBy(s => s.StartedAt!.Value.Date)                // null-guarded above
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Day.ToString("yyyy-MM-dd"), Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }

    /// <inheritdoc />
    public async Task<List<StepPercentileDto>> GetStepPercentilesAsync(CancellationToken ct)
    {
        // Navigate through FileRecord (which carries the global tenant+site query filter)
        // so we only read steps belonging to the current tenant/site.
        var stepData = await _db.Files
            .SelectMany(f => f.Steps, (_, s) => s)
            .Where(s => s.Status == "Success"
                     && s.StartedAt != null
                     && s.CompletedAt != null)
            .Select(s => new { s.StepName, s.StartedAt, s.CompletedAt })
            .ToListAsync(ct);

        if (!stepData.Any())
            return new List<StepPercentileDto>();

        return stepData
            .GroupBy(s => s.StepName)
            .Select(g =>
            {
                var durations = g
                    .Select(s => (s.CompletedAt!.Value - s.StartedAt!.Value).TotalSeconds)
                    .OrderBy(d => d)
                    .ToList();

                return new StepPercentileDto
                {
                    Step = g.Key,
                    SampleCount = durations.Count,
                    P50Seconds = Math.Round(Percentile(durations, 0.50), 1),
                    P90Seconds = Math.Round(Percentile(durations, 0.90), 1),
                    P99Seconds = Math.Round(Percentile(durations, 0.99), 1),
                };
            })
            .OrderBy(s => StepOrder(s.Step))
            .ToList();
    }

    // Linear interpolation (same as numpy/Excel PERCENTILE.INC)
    private static double Percentile(List<double> sorted, double p)
    {
        if (sorted.Count == 0) return 0;
        if (sorted.Count == 1) return sorted[0];

        double idx = p * (sorted.Count - 1);
        int lo = (int)Math.Floor(idx);
        int hi = (int)Math.Ceiling(idx);
        if (lo == hi) return sorted[lo];

        double frac = idx - lo;
        return sorted[lo] * (1 - frac) + sorted[hi] * frac;
    }

    private static int StepOrder(string step) => step switch
    {
        "Upload" => 0,
        "Validate" => 1,
        "Transform" => 2,
        "Load" => 3,
        _ => 99,
    };



}
