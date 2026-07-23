using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Storage;
using DocAnalytics.Service.Tests.Support;
using DocAnalytics.Service.Uploads;
using Moq;

namespace DocAnalytics.Service.Tests.Uploads;

public class UploadServiceTests
{
    private readonly TestCurrentUser _me = new() { TenantId = Guid.NewGuid(), SiteId = Guid.NewGuid() };
    private readonly Mock<IFileStorage> _storage = new();
    private readonly Mock<IExtractionQueue> _queue = new();

    public UploadServiceTests()
    {
        _storage.Setup(s => s.BuildKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns("acme/plant/2026/01/01/inv.pdf");
        _storage.Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://s3/put");
        _storage.Setup(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    }

    private UploadService Sut(AppDbContext db) => new(db, _me, _storage.Object, _queue.Object);

    private void SeedTenantSite(AppDbContext db)
    {
        db.Tenants.Add(new Tenant { Id = _me.TenantId, Name = "Acme", OrgDomain = "acme.com", IsActive = true, CreatedAt = DateTime.UtcNow });
        db.Sites.Add(new Site { Id = _me.SiteId, TenantId = _me.TenantId, Name = "Plant", IsActive = true, CreatedAt = DateTime.UtcNow });
        db.SaveChanges();
    }

    private Transaction SeedBatch(AppDbContext db)
    {
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            State = "Processing",
            SourceSystem = "Manual_Upload",
            TotalFiles = 1,
            SubmittedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
        db.Transactions.Add(txn);
        db.SaveChanges();
        return txn;
    }

    [Fact]
    public async Task CreateBatchAsync_throws_when_no_files()
    {
        using var db = InMemoryDb.Create(_me);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Sut(db).CreateBatchAsync(new CreateBatchRequest { FileCount = 0 }));
    }

    [Fact]
    public async Task CreateBatchAsync_creates_transaction_and_activity_log()
    {
        using var db = InMemoryDb.Create(_me);
        var res = await Sut(db).CreateBatchAsync(new CreateBatchRequest { FileCount = 3 });

        Assert.NotEqual(Guid.Empty, res.BatchId);
        Assert.Equal(3, db.Transactions.Single().TotalFiles);
        Assert.Equal(1, db.ActivityLog.Count());
    }

    [Fact]
    public async Task CreateUploadAsync_rejects_non_pdf()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Sut(db).CreateUploadAsync(new UploadUrlRequest { BatchId = batch.Id, FileName = "notes.txt", SizeBytes = 1000 }));
    }

    [Fact]
    public async Task CreateUploadAsync_rejects_oversize_file()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Sut(db).CreateUploadAsync(new UploadUrlRequest { BatchId = batch.Id, FileName = "big.pdf", SizeBytes = 20L * 1024 * 1024 }));
    }

    [Fact]
    public async Task CreateUploadAsync_creates_file_and_returns_url()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenantSite(db);
        var batch = SeedBatch(db);

        var res = await Sut(db).CreateUploadAsync(
            new UploadUrlRequest { BatchId = batch.Id, FileName = "inv.pdf", SizeBytes = 1000, OnDuplicate = null });

        Assert.Equal("https://s3/put", res.UploadUrl);
        Assert.NotEqual(Guid.Empty, res.FileId);
        Assert.Equal(1, db.Files.Count());
    }

    [Fact]
    public async Task CompleteAsync_enqueues_and_bumps_uploaded_count()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);
        var fileId = Guid.NewGuid();
        db.Files.Add(new FileRecord
        {
            Id = fileId,
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = batch.Id,
            FileName = "inv.pdf",
            FileType = "PDF",
            Status = "Queued",
            CurrentStep = "Upload",
            LastUpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        Assert.True(await Sut(db).CompleteAsync(fileId));
        _queue.Verify(q => q.EnqueueAsync(fileId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(1, db.Transactions.Single(t => t.Id == batch.Id).UploadedCount);
    }

    [Fact]
    public async Task GetDownloadUrlAsync_returns_null_when_file_missing()
    {
        using var db = InMemoryDb.Create(_me);
        Assert.Null(await Sut(db).GetDownloadUrlAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ShrinkBatchAsync_decrements_total_files()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);          // TotalFiles = 1... bump to 2 first
        batch.TotalFiles = 2; db.SaveChanges();

        Assert.True(await Sut(db).ShrinkBatchAsync(batch.Id));
        Assert.Equal(1, db.Transactions.Single(t => t.Id == batch.Id).TotalFiles);
    }

    [Fact]
    public async Task ShrinkBatchAsync_removes_empty_batch_when_no_files_left()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);          // TotalFiles = 1, no files attached
        Assert.True(await Sut(db).ShrinkBatchAsync(batch.Id));
        Assert.Empty(db.Transactions);      // 1 → 0 and no files → batch removed
    }

    [Fact]
    public async Task ShrinkBatchAsync_returns_false_when_missing()
    {
        using var db = InMemoryDb.Create(_me);
        Assert.False(await Sut(db).ShrinkBatchAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteBatchAsync_removes_batch_files_and_s3_objects()
    {
        using var db = InMemoryDb.Create(_me);
        var batch = SeedBatch(db);
        db.Files.Add(new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            TransactionId = batch.Id,
            FileName = "inv.pdf",
            FileType = "PDF",
            Status = "Completed",
            CurrentStep = "Load",
            StorageKey = "acme/plant/inv.pdf",
            LastUpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        Assert.True(await Sut(db).DeleteBatchAsync(batch.Id));
        Assert.Empty(db.Files);
        Assert.Empty(db.Transactions);
        _storage.Verify(s => s.DeleteAsync("acme/plant/inv.pdf", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBatchAsync_returns_false_when_missing()
    {
        using var db = InMemoryDb.Create(_me);
        Assert.False(await Sut(db).DeleteBatchAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateUploadAsync_accepts_jpg_file()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenantSite(db);
        var batch = SeedBatch(db);

        var res = await Sut(db).CreateUploadAsync(
            new UploadUrlRequest { BatchId = batch.Id, FileName = "scan.jpg", SizeBytes = 500_000 });

        Assert.Equal("https://s3/put", res.UploadUrl);
        Assert.NotEqual(Guid.Empty, res.FileId);
    }

    [Fact]
    public async Task CreateUploadAsync_accepts_jpeg_extension()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenantSite(db);
        var batch = SeedBatch(db);

        var res = await Sut(db).CreateUploadAsync(
            new UploadUrlRequest { BatchId = batch.Id, FileName = "receipt.jpeg", SizeBytes = 300_000 });

        Assert.NotEqual(Guid.Empty, res.FileId);
    }


}
