using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Realtime;
using DocAnalytics.Service.Tests.Support;
using Moq;

namespace DocAnalytics.Service.Tests.Realtime;

public class SimulationServiceTests
{
    private readonly TestCurrentUser _me = new() { TenantId = Guid.NewGuid(), SiteId = Guid.NewGuid() };
    private readonly Mock<IPipelineNotifier> _notifier = new();

    private SimulationService Sut(AppDbContext db) => new(db, _me, _notifier.Object);

    [Fact]
    public async Task SimulateStateChangeAsync_returns_null_when_no_files()
    {
        using var db = InMemoryDb.Create(_me);
        Assert.Null(await Sut(db).SimulateStateChangeAsync());
    }

    [Fact]
    public async Task SimulateStateChangeAsync_flips_state_logs_and_notifies()
    {
        using var db = InMemoryDb.Create(_me);
        var txnId = Guid.NewGuid();
        db.Transactions.Add(new Transaction
        {
            Id = txnId,
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            State = "Processing",
            SourceSystem = "Manual_Upload",
            TotalFiles = 1,
            ProcessingCount = 1,
            SubmittedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });
        db.Files.Add(new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = txnId,
            FileName = "f.pdf",
            FileType = "PDF",
            Status = "Processing",
            CurrentStep = "Validate",
            LastUpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        var result = await Sut(db).SimulateStateChangeAsync();

        Assert.NotNull(result);
        Assert.Contains(result!.NewState, new[] { "Completed", "Failed" });
        Assert.Equal(1, db.ActivityLog.Count());   // audit row written
        _notifier.Verify(n => n.NotifyFileStateChangedAsync(
            _me.SiteId, It.IsAny<FileStateChangedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
