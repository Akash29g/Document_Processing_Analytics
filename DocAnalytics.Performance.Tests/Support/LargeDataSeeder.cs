using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Performance.Tests.Support;

// Generates a large in-memory dataset simulating production scale (NFR-1: up to 1M files).
// Default is 100k for CI-friendly runtimes; bump TOTAL_FILES to 1_000_000 for a full run.
public static class LargeDataSeeder
{
    public const int TotalBatches = 2_000;
    public const int FilesPerBatch = 50;          // 2,000 x 50 = 100,000 files
    private static readonly string[] Steps = { "Upload", "Validate", "Transform", "Load" };

    public static void Seed(AppDbContext db, Guid tenantId, Guid siteId)
    {
        var rng = new Random(42);   // fixed seed = reproducible timings
        var now = DateTime.UtcNow;

        var transactions = new List<Transaction>(TotalBatches);
        var files = new List<FileRecord>(TotalBatches * FilesPerBatch);
        var steps = new List<FileStepHistory>();

        for (var b = 0; b < TotalBatches; b++)
        {
            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                SiteId = siteId,
                SourceSystem = b % 2 == 0 ? "SAP" : "CSV",
                State = b % 10 == 0 ? "Failed" : "Completed",
                TotalFiles = FilesPerBatch,
                CompletedCount = FilesPerBatch - 5,
                FailedCount = 3,
                ProcessingCount = 1,
                UploadedCount = 1,
                SubmittedAt = now.AddDays(-rng.Next(0, 30)),
                LastUpdatedAt = now
            };
            transactions.Add(tx);

            for (var f = 0; f < FilesPerBatch; f++)
            {
                var failed = f % 20 == 0;   // 5% failure rate
                var file = new FileRecord
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    SiteId = siteId,
                    TransactionId = tx.Id,
                    FileName = $"file_{b}_{f}.pdf",
                    FileType = "pdf",
                    Status = failed ? "Failed" : "Completed",
                    CurrentStep = failed ? "Validate" : "Load",
                    LastUpdatedAt = now.AddMinutes(-rng.Next(0, 10_000))
                };
                files.Add(file);

                if (failed)
                {
                    steps.Add(new FileStepHistory
                    {
                        Id = Guid.NewGuid(),
                        FileId = file.Id,
                        StepName = "Validate",
                        Status = "Failed",
                        ErrorCode = "ERR_SCHEMA",
                        ErrorMessage = "Schema validation failed",
                        StartedAt = now.AddMinutes(-5),
                        CompletedAt = now.AddMinutes(-4)
                    });
                }
            }
        }

        db.Transactions.AddRange(transactions);
        db.Files.AddRange(files);
        db.FileStepHistory.AddRange(steps);
        db.SaveChanges();
    }
}
