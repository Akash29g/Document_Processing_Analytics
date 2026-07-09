using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Common;
using DocAnalytics.Service.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class DashboardControllerTests
{
    [Fact]
    public async Task GetSummary_returns_200_with_data()
    {
        var svc = new Mock<IDashboardService>();
        svc.Setup(s => s.GetSummaryAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new DashboardSummaryResponse { Total = 10, Completed = 6 });

        var result = await new DashboardController(svc.Object).GetSummary(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<DashboardSummaryResponse>>(ok.Value);
        Assert.Equal(10, body.Data!.Total);
    }

    [Fact]
    public async Task GetRecentFailures_returns_200_with_list_and_meta()
    {
        var paged = new PagedResult<RecentFailureDto>
        {
            Items = new() { new RecentFailureDto { FileName = "a.pdf", FailedStep = "Validate" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        var svc = new Mock<IDashboardService>();
        svc.Setup(s => s.GetRecentFailuresAsync(It.IsAny<RecentFailuresQuery>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(paged);

        var result = await new DashboardController(svc.Object).GetRecentFailures(new RecentFailuresQuery(), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<List<RecentFailureDto>>>(ok.Value);
        Assert.Single(body.Data!);
        Assert.Equal(1, body.Meta!.TotalCount);
    }
}
