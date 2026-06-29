using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Analytics;



public sealed class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;
    public AnalyticsService(AppDbContext db) => _db = db;

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


}
