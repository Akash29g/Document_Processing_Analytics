using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using ErrorCatalogEntry = DocAnalytics.Domain.Entities.ErrorCatalog;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Files;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;



namespace DocAnalytics.Service.Tests.Files;

public class FileDetailsServiceTests
{
    // ── helpers ────────────────────────────────────────────────────────────

    private static Mock<DocAnalytics.Data.AppDbContext> Ctx(
        FileRecord[] files, FileStepHistory[] steps, ErrorCatalogEntry[] catalog)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.FileStepHistory).Returns(steps.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.ErrorCatalog).Returns(catalog.ToList().BuildMockDbSet().Object);
        return ctx;
    }

    // Wrap construction so existing tests don't need to know about the new deps
    // (IExtractionQueue + ICurrentUser are only used by RetryFileAsync, not these tests)
    private static FileDetailsService Svc(Mock<DocAnalytics.Data.AppDbContext> ctx) =>
        new(ctx.Object,
            new Mock<IExtractionQueue>().Object,
            new Mock<ICurrentUser>().Object);

    // ── tests ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetFileDetailsAsync_returns_null_when_file_missing()
    {
        var sut = Svc(Ctx(Array.Empty<FileRecord>(),
                          Array.Empty<FileStepHistory>(),
                          Array.Empty<ErrorCatalogEntry>()));
        Assert.Null(await sut.GetFileDetailsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetFileDetailsAsync_maps_history_with_remediation()
    {
        var fileId = Guid.NewGuid();
        var files = new[]
        {
            new FileRecord
            {
                Id = fileId, FileName = "a.pdf",
                Status = "Failed", CurrentStep = "Validate",
            },
        };
        var steps = new[]
        {
            new FileStepHistory
            {
                Id = Guid.NewGuid(), FileId = fileId, StepName = "Upload",
                Status = "Success", StartedAt = DateTime.UtcNow.AddMinutes(-2),
            },
            new FileStepHistory
            {
                Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate",
                Status = "Failed", ErrorCode = "E1", ErrorMessage = "bad",
                StartedAt = DateTime.UtcNow.AddMinutes(-1),
            },
        };
        var catalog = new[] { new ErrorCatalogEntry { ErrorCode = "E1", RemediationMsg = "Fix it" } };

        var dto = await Svc(Ctx(files, steps, catalog)).GetFileDetailsAsync(fileId);

        Assert.NotNull(dto);
        Assert.Equal("a.pdf", dto!.FileInfo.Name);
        Assert.Equal(2, dto.History.Count);
        var failed = dto.History.Single(h => h.Step == "Validate");
        Assert.Equal("Fix it", failed.Error!.SuggestedFix);
        Assert.Null(dto.History.Single(h => h.Step == "Upload").Error);
    }

    [Fact]
    public async Task GetFileLogsAsync_returns_null_when_file_missing()
    {
        var sut = Svc(Ctx(Array.Empty<FileRecord>(),
                          Array.Empty<FileStepHistory>(),
                          Array.Empty<ErrorCatalogEntry>()));
        Assert.Null(await sut.GetFileLogsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetFileLogsAsync_builds_downloadable_log()
    {
        var fileId = Guid.NewGuid();
        var files = new[]
        {
            new FileRecord
            {
                Id = fileId, FileName = "a.pdf",
                Status = "Failed", CurrentStep = "Validate",
            },
        };
        var steps = new[]
        {
            new FileStepHistory
            {
                Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate",
                Status = "Failed", ErrorCode = "E1", ErrorMessage = "bad",
                StartedAt = DateTime.UtcNow,
            },
        };
        var catalog = new[] { new ErrorCatalogEntry { ErrorCode = "E1", RemediationMsg = "Fix it" } };

        var log = await Svc(Ctx(files, steps, catalog)).GetFileLogsAsync(fileId);

        Assert.NotNull(log);
        Assert.Equal($"file_{fileId}_log.txt", log!.FileName);
        Assert.Contains("Validate", log.Content);
        Assert.Contains("Fix it", log.Content);
    }
}
