using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Charts;

public sealed class ChartService : IChartService
{
    private readonly AppDbContext _db;
    public ChartService(AppDbContext db) => _db = db;

    public async Task<ChartSeriesDto> GetStatusDistributionAsync(CancellationToken ct = default)
    {
        var points = await _db.Files
            .AsNoTracking()
            .GroupBy(f => f.Status)                    // bucket files by their Status
            .Select(g => new ChartPointDto
            {
                Label = g.Key,                         // the status value, e.g. "Completed"
                Value = g.LongCount()                  // COUNT(*) for that status
            })
            .OrderByDescending(p => p.Value)           // biggest slice first
            .ThenBy(p => p.Label)                      // tiebreaker → deterministic order
            .ToListAsync(ct);

        return new ChartSeriesDto { Points = points };
    }

    public async Task<ChartSeriesDto> GetThroughputAsync(CancellationToken ct = default)
    {
        // PHASE 1 — DB does the work: only COMPLETED files, bucketed by the day they finished.
        // FR-1.2: throughput = files *completed* per day (not files uploaded).
        // NOTE: FileRecord has no completed_at, so LastUpdatedAt is the closest completion signal.
        var raw = await _db.Files
            .AsNoTracking()
            .Where(f => f.Status == "Completed")      // ✅ completed only (PascalCase matches seeder)
            .GroupBy(f => f.LastUpdatedAt.Date)        // ✅ bucket by completion day
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        // PHASE 2 — format labels in memory (tiny: one row per day).
        var points = raw
            .Select(x => new ChartPointDto
            {
                Label = x.Day.ToString("yyyy-MM-dd"),
                Value = x.Count
            })
            .ToList();

        return new ChartSeriesDto { Points = points };
    }


    public async Task<ChartSeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default)
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
            .Select(x => new ChartPointDto { Label = x.Code, Value = x.Count })
            .ToList();

        return new ChartSeriesDto { Points = points };
    }
    public async Task<ChartSeriesDto> GetErrorTrendAsync(CancellationToken ct = default)
    {
        var raw = await _db.Files                                  // ① anchor on scoped table
            .AsNoTracking()
            .SelectMany(f => f.Steps)                             // ② out to non-scoped steps
            .Where(s => s.ErrorCode != null && s.StartedAt != null) // ③ errored AND has a timestamp
            .GroupBy(s => s.StartedAt!.Value.Date)                // ④ bucket by day (null-guarded)
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)                                  // ⑤ chronological
            .ToListAsync(ct);

        var points = raw
            .Select(x => new ChartPointDto
            {
                Label = x.Day.ToString("yyyy-MM-dd"),             // ⑥ format in memory
                Value = x.Count
            })
            .ToList();

        return new ChartSeriesDto { Points = points };
    }


}
