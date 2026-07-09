using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class DashboardAnalyticsControllerTests
{
    [Fact]
    public async Task GetStatusDistribution_returns_200()
    {
        var svc = new Mock<IAnalyticsService>();
        svc.Setup(s => s.GetStatusDistributionAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new SeriesDto { Points = new() { new SeriesPointDto { Label = "Completed", Value = 5 } } });

        var ok = Assert.IsType<OkObjectResult>(await new DashboardAnalyticsController(svc.Object).GetStatusDistribution(default));
        var body = Assert.IsType<ApiResponse<SeriesDto>>(ok.Value);
        Assert.Single(body.Data!.Points);
    }

    [Fact]
    public async Task GetThroughput_returns_200()
    {
        var svc = new Mock<IAnalyticsService>();
        svc.Setup(s => s.GetThroughputAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new SeriesDto { Points = new() });

        Assert.IsType<OkObjectResult>(await new DashboardAnalyticsController(svc.Object).GetThroughput(new AnalyticsRangeQuery(), default));
    }

    [Fact]
    public async Task GetStepPercentiles_returns_200()
    {
        var svc = new Mock<IAnalyticsService>();
        svc.Setup(s => s.GetStepPercentilesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<StepPercentileDto> { new() { Step = "Upload", SampleCount = 3 } });

        var ok = Assert.IsType<OkObjectResult>(await new DashboardAnalyticsController(svc.Object).GetStepPercentiles(default));
        var body = Assert.IsType<ApiResponse<List<StepPercentileDto>>>(ok.Value);
        Assert.Single(body.Data!);
    }
}
