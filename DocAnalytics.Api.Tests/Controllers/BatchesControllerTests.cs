using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class BatchesControllerTests
{
    [Fact]
    public async Task GetBatches_returns_200_with_list_and_meta()
    {
        var paged = new PagedResult<BatchListItemDto>
        {
            Items = new() { new BatchListItemDto { TransactionId = Guid.NewGuid(), State = "Completed", SourceSystem = "SAP" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetBatchesAsync(It.IsAny<BatchListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var ok = Assert.IsType<OkObjectResult>(await new BatchesController(svc.Object).GetBatches(new BatchListQuery(), default));
        var body = Assert.IsType<ApiResponse<List<BatchListItemDto>>>(ok.Value);
        Assert.Single(body.Data!);
        Assert.Equal(1, body.Meta!.TotalCount);
    }

    [Fact]
    public async Task GetSources_returns_200_with_list()
    {
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetSourcesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string> { "CSV", "SAP" });

        var ok = Assert.IsType<OkObjectResult>(await new BatchesController(svc.Object).GetSources(default));
        var body = Assert.IsType<ApiResponse<List<string>>>(ok.Value);
        Assert.Equal(2, body.Data!.Count);
    }

    [Fact]
    public async Task GetBatchById_returns_404_when_missing()
    {
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetBatchByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((BatchDetailDto?)null);

        var nf = Assert.IsType<NotFoundObjectResult>(await new BatchesController(svc.Object).GetBatchById(Guid.NewGuid(), default));
        var body = Assert.IsType<ApiResponse<BatchDetailDto>>(nf.Value);
        Assert.Equal("not_found", body.Error!.Code);
    }

    [Fact]
    public async Task GetBatchById_returns_200_when_found()
    {
        var dto = new BatchDetailDto { Id = Guid.NewGuid(), Status = "Failed", Source = "SAP", FileStats = new FileStatsDto(), Times = new BatchTimesDto() };
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetBatchByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var ok = Assert.IsType<OkObjectResult>(await new BatchesController(svc.Object).GetBatchById(Guid.NewGuid(), default));
        var body = Assert.IsType<ApiResponse<BatchDetailDto>>(ok.Value);
        Assert.Equal("Failed", body.Data!.Status);
    }

    [Fact]
    public async Task GetBatchFiles_returns_404_when_missing()
    {
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetBatchFilesAsync(It.IsAny<Guid>(), It.IsAny<BatchFilesQuery>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((PagedResult<BatchFileDto>?)null);

        var nf = Assert.IsType<NotFoundObjectResult>(await new BatchesController(svc.Object).GetBatchFiles(Guid.NewGuid(), new BatchFilesQuery(), default));
        var body = Assert.IsType<ApiResponse<List<BatchFileDto>>>(nf.Value);
        Assert.Equal("not_found", body.Error!.Code);
    }

    [Fact]
    public async Task GetBatchFiles_returns_200_with_list_and_meta()
    {
        var paged = new PagedResult<BatchFileDto>
        {
            Items = new() { new BatchFileDto { Id = Guid.NewGuid(), FileName = "a.pdf", FileType = "pdf", Status = "Completed", CurrentStep = "Load" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        var svc = new Mock<IBatchService>();
        svc.Setup(s => s.GetBatchFilesAsync(It.IsAny<Guid>(), It.IsAny<BatchFilesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var ok = Assert.IsType<OkObjectResult>(await new BatchesController(svc.Object).GetBatchFiles(Guid.NewGuid(), new BatchFilesQuery(), default));
        var body = Assert.IsType<ApiResponse<List<BatchFileDto>>>(ok.Value);
        Assert.Single(body.Data!);
        Assert.Equal(1, body.Meta!.TotalCount);
    }
}
