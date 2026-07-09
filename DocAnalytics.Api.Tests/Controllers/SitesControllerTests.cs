using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class SitesControllerTests
{
    [Fact]
    public async Task GetSites_returns_200_with_sites()
    {
        var userId = Guid.NewGuid();
        IReadOnlyList<SiteDto> sites = new List<SiteDto> { new(Guid.NewGuid(), "Plant One") };
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(userId);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.GetSitesAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(sites);

        var result = await new SitesController(auth.Object, currentUser.Object).GetSites(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<IReadOnlyList<SiteDto>>>(ok.Value);
        Assert.Single(body.Data!);
    }
}
