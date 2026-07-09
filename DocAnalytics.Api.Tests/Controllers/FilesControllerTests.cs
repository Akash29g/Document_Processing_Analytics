using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Files;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class FilesControllerTests
{
    [Fact]
    public async Task GetDetails_returns_404_when_missing()
    {
        var svc = new Mock<IFileDetailsService>();
        svc.Setup(s => s.GetFileDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((FileDetailDto?)null);

        var result = await new FilesController(svc.Object).GetDetails(Guid.NewGuid(), default);

        var nf = Assert.IsType<NotFoundObjectResult>(result);
        var body = Assert.IsType<ApiResponse<FileDetailDto>>(nf.Value);
        Assert.Equal("NOT_FOUND", body.Error!.Code);
    }

    [Fact]
    public async Task GetDetails_returns_200_when_found()
    {
        var dto = new FileDetailDto { FileInfo = new FileInfoDto { Id = Guid.NewGuid(), Name = "a.pdf", CurrentStatus = "Failed", CurrentStep = "Validate" } };
        var svc = new Mock<IFileDetailsService>();
        svc.Setup(s => s.GetFileDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await new FilesController(svc.Object).GetDetails(Guid.NewGuid(), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<FileDetailDto>>(ok.Value);
        Assert.Equal("a.pdf", body.Data!.FileInfo.Name);
    }

    [Fact]
    public async Task GetLogs_returns_404_when_missing()
    {
        var svc = new Mock<IFileDetailsService>();
        svc.Setup(s => s.GetFileLogsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((FileLogDto?)null);

        Assert.IsType<NotFoundObjectResult>(await new FilesController(svc.Object).GetLogs(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task GetLogs_returns_text_file_when_found()
    {
        var log = new FileLogDto { FileName = "file_x_log.txt", Content = "hello" };
        var svc = new Mock<IFileDetailsService>();
        svc.Setup(s => s.GetFileLogsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(log);

        var file = Assert.IsType<FileContentResult>(await new FilesController(svc.Object).GetLogs(Guid.NewGuid(), default));
        Assert.Equal("text/plain", file.ContentType);
        Assert.Equal("file_x_log.txt", file.FileDownloadName);
    }
}
