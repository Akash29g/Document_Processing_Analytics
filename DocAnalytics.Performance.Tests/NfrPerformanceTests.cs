using System.Diagnostics;
using DocAnalytics.Data;
using DocAnalytics.Performance.Tests.Support;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Service.Errors;

namespace DocAnalytics.Performance.Tests;

// One shared, seeded database for the whole class (seeding 100k files once, not per test).
public sealed class PerfFixture : IDisposable
{
    public readonly Guid TenantId = Guid.NewGuid();
    public readonly Guid SiteId = Guid.NewGuid();
    public AppDbContext Db { get; }

    private readonly string _dbName = $"perf-{Guid.NewGuid()}";

    public PerfFixture()
    {
        Db = PerfDb.Create(new PerfCurrentUser { TenantId = TenantId, SiteId = SiteId }, _dbName);
        LargeDataSeeder.Seed(Db, TenantId, SiteId);
    }

    // Each concurrent "user" needs its own DbContext (contexts are not thread-safe) —
    // a new context pointing at the SAME named in-memory database.
    public AppDbContext NewSession() =>
        PerfDb.Create(new PerfCurrentUser { TenantId = TenantId, SiteId = SiteId }, _dbName);

    public void Dispose()
    {
        PerfReport.Write("../../../../perf-results");   // repo-root/perf-results
        Db.Dispose();
    }
}

public class NfrPerformanceTests : IClassFixture<PerfFixture>
{
    private const int Runs = 10;                      // samples per operation for P50/P90
    private static readonly TimeSpan DashboardBudget = TimeSpan.FromSeconds(3); // NFR-1
    private static readonly TimeSpan ListBudget = TimeSpan.FromSeconds(1);      // NFR-1

    private readonly PerfFixture _fx;
    public NfrPerformanceTests(PerfFixture fx) => _fx = fx;

    private static async Task<TimeSpan> TimeAsync(string op, Func<Task> action)
    {
        var sw = Stopwatch.StartNew();
        await action();
        sw.Stop();
        PerfReport.Record(op, sw.Elapsed.TotalMilliseconds);
        return sw.Elapsed;
    }

    [Fact]
    public async Task Dashboard_summary_stays_under_3s_at_scale()
    {
        var svc = new DashboardService(_fx.Db);
        for (var i = 0; i < Runs; i++)
        {
            var elapsed = await TimeAsync("dashboard_summary", () => svc.GetSummaryAsync());
            Assert.True(elapsed < DashboardBudget,
                $"Dashboard summary took {elapsed.TotalMilliseconds:F0}ms (budget {DashboardBudget.TotalMilliseconds}ms)");
        }
    }

    [Fact]
    public async Task Recent_failures_page_stays_under_1s_at_scale()
    {
        var svc = new DashboardService(_fx.Db);
        var query = new RecentFailuresQuery { Page = 1, PageSize = 50 };
        for (var i = 0; i < Runs; i++)
        {
            var elapsed = await TimeAsync("recent_failures_page", () => svc.GetRecentFailuresAsync(query));
            Assert.True(elapsed < ListBudget,
                $"Recent failures took {elapsed.TotalMilliseconds:F0}ms (budget {ListBudget.TotalMilliseconds}ms)");
        }
    }

    [Fact]
    public async Task Batch_list_page_stays_under_1s_at_scale()
    {
        var svc = new BatchService(_fx.Db);
        var query = new BatchListQuery { Page = 1, PageSize = 50 };
        for (var i = 0; i < Runs; i++)
        {
            var elapsed = await TimeAsync("batch_list_page", () => svc.GetBatchesAsync(query));
            Assert.True(elapsed < ListBudget,
                $"Batch list took {elapsed.TotalMilliseconds:F0}ms (budget {ListBudget.TotalMilliseconds}ms)");
        }
    }

    [Fact]
    public async Task Error_list_page_stays_under_1s_at_scale()
    {
        var svc = new ErrorService(_fx.Db);
        var query = new ErrorListQuery { Page = 1, PageSize = 50 };
        for (var i = 0; i < Runs; i++)
        {
            var elapsed = await TimeAsync("error_list_page", () => svc.GetErrorsAsync(query));
            Assert.True(elapsed < ListBudget,
                $"Error list took {elapsed.TotalMilliseconds:F0}ms (budget {ListBudget.TotalMilliseconds}ms)");
        }
    }

    [Fact]
    public async Task Ten_concurrent_users_no_degradation()
    {
        // NFR-1: 10 concurrent users. Each gets its own DbContext session (thread safety).
        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, 10).Select(async _ =>
        {
            using var db = _fx.NewSession();
            var dash = new DashboardService(db);
            var elapsed = await TimeAsync("concurrent_dashboard_summary", () => dash.GetSummaryAsync());
            Assert.True(elapsed < DashboardBudget,
                $"Concurrent summary took {elapsed.TotalMilliseconds:F0}ms (budget {DashboardBudget.TotalMilliseconds}ms)");
        });

        await Task.WhenAll(tasks);
        sw.Stop();
        PerfReport.Record("concurrent_total_wall_time", sw.Elapsed.TotalMilliseconds);
    }
}
