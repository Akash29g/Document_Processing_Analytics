using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Files;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Files;

public class FileDetailsServiceTests
{
    private static Mock<DocAnalytics.Data.AppDbContext> Ctx(
        FileRecord[] files, FileStepHistory[] steps, ErrorCatalog[] catalog)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.BuildMockDbSet().Object);
        ctx.Setup(c => c.FileStepHistory).Returns(steps.BuildMockDbSet().Object);
        ctx.Setup(c => c.ErrorCatalog).Returns(catalog.BuildMockDbSet().Object);
        return ctx;
    }

    [Fact]
    public async Task GetFileDetailsAsync_returns_null_when_file_missing()
    {
        var sut = new FileDetailsService(Ctx(Array.Empty<FileRecord>(), Array.Empty<FileStepHistory>(), Array.Empty<ErrorCatalog>()).Object);
        Assert.Null(await sut.GetFileDetailsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetFileDetailsAsync_maps_history_with_remediation()
    {
        var fileId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", Status = "Failed", CurrentStep = "Validate" } };
        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Upload",   Status = "Success", StartedAt = DateTime.UtcNow.AddMinutes(-2) },
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed",  ErrorCode = "E1", ErrorMessage = "bad", StartedAt = DateTime.UtcNow.AddMinutes(-1) },
        };
        var catalog = new[] { new ErrorCatalog { ErrorCode = "E1", RemediationMsg = "Fix it" } };

        var dto = await new FileDetailsService(Ctx(files, steps, catalog).Object).GetFileDetailsAsync(fileId);

        Assert.NotNull(dto);
        Assert.Equal("a.pdf", dto!.FileInfo.Name);
        Assert.Equal(2, dto.History.Count);
        var failed = dto.History.Single(h => h.Step == "Validate");
        Assert.Equal("Fix it", failed.Error!.SuggestedFix);
        Assert.Null(dto.History.Single(h => h.Step == "Upload").Error);   // success → no error block
    }

    [Fact]
    public async Task GetFileLogsAsync_returns_null_when_file_missing()
    {
        var sut = new FileDetailsService(Ctx(Array.Empty<FileRecord>(), Array.Empty<FileStepHistory>(), Array.Empty<ErrorCatalog>()).Object);
        Assert.Null(await sut.GetFileLogsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetFileLogsAsync_builds_downloadable_log()
    {
        var fileId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", Status = "Failed", CurrentStep = "Validate" } };
        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1", ErrorMessage = "bad", StartedAt = DateTime.UtcNow },
        };
        var catalog = new[] { new ErrorCatalog { ErrorCode = "E1", RemediationMsg = "Fix it" } };

        var log = await new FileDetailsService(Ctx(files, steps, catalog).Object).GetFileLogsAsync(fileId);

        Assert.NotNull(log);
        Assert.Equal($"file_{fileId}_log.txt", log!.FileName);
        Assert.Contains("Validate", log.Content);
        Assert.Contains("Fix it", log.Content);
    }
}
