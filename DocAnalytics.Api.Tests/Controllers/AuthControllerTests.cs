using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_returns_200_with_envelope_on_success()
    {
        var response = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>());
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await new AuthController(auth.Object, Mock.Of<ICurrentUser>())
            .Login(new LoginRequest("a@org.com", "pw"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<LoginResponse>>(ok.Value);
        Assert.Equal("jwt", body.Data!.Token);
    }

    [Fact]
    public async Task Login_returns_401_on_invalid_credentials()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((LoginResponse?)null);

        var result = await new AuthController(auth.Object, Mock.Of<ICurrentUser>())
            .Login(new LoginRequest("a@org.com", "bad"), default);

        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = Assert.IsType<ApiResponse<object>>(unauth.Value);
        Assert.Equal("INVALID_CREDENTIALS", body.Error!.Code);
    }

    [Fact]
    public async Task Me_returns_401_when_user_not_found()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(Guid.NewGuid());
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.GetMeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((MeResponse?)null);

        var result = await new AuthController(auth.Object, currentUser.Object).Me(default);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Me_returns_200_with_user_and_sites()
    {
        var userId = Guid.NewGuid();
        var me = new MeResponse(new UserDto(userId, "a@org.com", "Admin"), new List<SiteDto>());
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(userId);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.GetMeAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(me);

        var result = await new AuthController(auth.Object, currentUser.Object).Me(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<MeResponse>>(ok.Value);
        Assert.Equal("a@org.com", body.Data!.User.Email);
    }
}
