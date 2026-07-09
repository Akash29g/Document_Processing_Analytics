using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Common;
using DocAnalytics.Service.Errors;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class ErrorsControllerTests
{
    [Fact]
    public async Task GetErrors_returns_200_with_list_and_meta()
    {
        var paged = new PagedResult<ErrorListItemDto>
        {
            Items = new() { new ErrorListItemDto { FileName = "a.pdf", ErrorCode = "E1", Step = "Validate", Source = "SAP" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        var svc = new Mock<IErrorService>();
        svc.Setup(s => s.GetErrorsAsync(It.IsAny<ErrorListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await new ErrorsController(svc.Object).GetErrors(new ErrorListQuery(), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<List<ErrorListItemDto>>>(ok.Value);
        Assert.Single(body.Data!);
        Assert.Equal(1, body.Meta!.TotalCount);
    }

    [Fact]
    public async Task ExportErrors_returns_csv_file()
    {
        var rows = new List<ErrorListItemDto>
        {
            new() { FileId = Guid.NewGuid(), FileName = "a.pdf", ErrorCode = "E1", Step = "Validate", Source = "SAP" }
        };
        var svc = new Mock<IErrorService>();
        svc.Setup(s => s.GetErrorsForExportAsync(It.IsAny<ErrorListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(rows);

        var result = await new ErrorsController(svc.Object).ExportErrors(new ErrorListQuery(), default);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", file.ContentType);
        Assert.Contains("errors_export", file.FileDownloadName);
        Assert.NotEmpty(file.FileContents);
    }
}
