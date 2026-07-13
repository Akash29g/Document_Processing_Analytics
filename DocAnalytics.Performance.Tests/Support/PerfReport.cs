using System.Text;

namespace DocAnalytics.Performance.Tests.Support;

// Collects timing samples and writes a small markdown + CSV report (P50/P90),
// reusing the step-percentiles idea from the dashboard.
public static class PerfReport
{
    private static readonly object Gate = new();
    private static readonly Dictionary<string, List<double>> Samples = new();

    public static void Record(string operation, double elapsedMs)
    {
        lock (Gate)
        {
            if (!Samples.TryGetValue(operation, out var list))
                Samples[operation] = list = new List<double>();
            list.Add(elapsedMs);
        }
    }

    public static void Write(string directory)
    {
        lock (Gate)
        {
            Directory.CreateDirectory(directory);
            var md = new StringBuilder();
            var csv = new StringBuilder();

            md.AppendLine("# Performance Report (mocked, in-memory)");
            md.AppendLine();
            md.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}  ");
            md.AppendLine($"Dataset: {LargeDataSeeder.TotalBatches:N0} batches x {LargeDataSeeder.FilesPerBatch} files");
            md.AppendLine();
            md.AppendLine("| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |");
            md.AppendLine("|---|---:|---:|---:|---:|");
            csv.AppendLine("operation,samples,p50_ms,p90_ms,max_ms");

            foreach (var (op, list) in Samples.OrderBy(kv => kv.Key))
            {
                var sorted = list.OrderBy(x => x).ToList();
                var p50 = Percentile(sorted, 0.50);
                var p90 = Percentile(sorted, 0.90);
                var max = sorted[^1];

                md.AppendLine($"| {op} | {sorted.Count} | {p50:F1} | {p90:F1} | {max:F1} |");
                csv.AppendLine($"{op},{sorted.Count},{p50:F1},{p90:F1},{max:F1}");
            }

            File.WriteAllText(Path.Combine(directory, "perf-report.md"), md.ToString());
            File.WriteAllText(Path.Combine(directory, "perf-report.csv"), csv.ToString());
        }
    }

    private static double Percentile(List<double> sorted, double p)
    {
        if (sorted.Count == 0) return 0;
        var idx = (int)Math.Ceiling(p * sorted.Count) - 1;
        return sorted[Math.Clamp(idx, 0, sorted.Count - 1)];
    }
}
