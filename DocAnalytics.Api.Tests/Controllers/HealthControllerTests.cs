using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Health;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class HealthControllerTests
{
    [Fact]
    public async Task Get_returns_200_when_db_reachable()
    {
        var svc = new Mock<IHealthService>();
        svc.Setup(s => s.IsDatabaseReachableAsync()).ReturnsAsync(true);

        Assert.IsType<OkObjectResult>(await new HealthController(svc.Object).Get());
    }

    [Fact]
    public async Task Get_returns_503_when_db_unreachable()
    {
        var svc = new Mock<IHealthService>();
        svc.Setup(s => s.IsDatabaseReachableAsync()).ReturnsAsync(false);

        var obj = Assert.IsType<ObjectResult>(await new HealthController(svc.Object).Get());
        Assert.Equal(503, obj.StatusCode);
    }
}
