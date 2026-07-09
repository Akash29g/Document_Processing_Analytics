using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class ErrorAnalyticsControllerTests
{
    [Fact]
    public async Task GetTopErrors_returns_200()
    {
        var series = new SeriesDto { Points = new() { new SeriesPointDto { Label = "E1", Value = 3 } } };
        var svc = new Mock<IAnalyticsService>();
        svc.Setup(s => s.GetTopErrorsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(series);

        var result = await new ErrorAnalyticsController(svc.Object).GetTopErrors(5, default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<SeriesDto>>(ok.Value);
        Assert.Single(body.Data!.Points);
    }

    [Fact]
    public async Task GetErrorTrend_returns_200()
    {
        var svc = new Mock<IAnalyticsService>();
        svc.Setup(s => s.GetErrorTrendAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new SeriesDto { Points = new() });

        var result = await new ErrorAnalyticsController(svc.Object).GetErrorTrend(new AnalyticsRangeQuery(), default);

        Assert.IsType<OkObjectResult>(result);
    }
}
