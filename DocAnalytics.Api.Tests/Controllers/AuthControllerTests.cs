using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class AuthControllerTests
{
    // Login touches HttpContext.Connection / Response — give the controller a real context.
    // refresh/jwt default to bare mocks so existing tests don't need to pass them.
    private static AuthController NewController(
        IAuthService auth, ICurrentUser user, ILoginLockoutService lockout,
        IRefreshTokenService? refresh = null, IJwtTokenService? jwt = null)
        => new(auth, user, lockout,
               refresh ?? Mock.Of<IRefreshTokenService>(),
               jwt ?? Mock.Of<IJwtTokenService>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

    [Fact]
    public async Task Login_returns_200_with_envelope_on_success()
    {
        var response = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>(), false);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
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

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .Login(new LoginRequest("a@org.com", "bad"), default);

        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = Assert.IsType<ApiResponse<object>>(unauth.Value);
        Assert.Equal("INVALID_CREDENTIALS", body.Error!.Code);
    }

    [Fact]
    public async Task Login_returns_429_when_account_locked()
    {
        var auth = new Mock<IAuthService>();
        var lockout = new Mock<ILoginLockoutService>();
        lockout.Setup(l => l.IsLockedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((true, 120));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), lockout.Object)
            .Login(new LoginRequest("a@org.com", "pw"), default);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, obj.StatusCode);
        var body = Assert.IsType<ApiResponse<object>>(obj.Value);
        Assert.Equal("RATE_LIMITED", body.Error!.Code);
        // Locked out BEFORE credentials are ever checked.
        auth.Verify(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Login_registers_failure_on_bad_password()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((LoginResponse?)null);
        var lockout = new Mock<ILoginLockoutService>();
        lockout.Setup(l => l.IsLockedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((false, 0));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), lockout.Object)
            .Login(new LoginRequest("a@org.com", "bad"), default);

        Assert.IsType<UnauthorizedObjectResult>(result);
        lockout.Verify(l => l.RegisterFailureAsync("a@org.com", It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_issues_refresh_token_on_success()
    {
        var response = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>(), false);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.IssueAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(("raw-refresh", DateTime.UtcNow.AddDays(7)));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object)
            .Login(new LoginRequest("a@org.com", "pw"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<LoginResponse>>(ok.Value);
        Assert.Equal("raw-refresh", body.Data!.RefreshToken);
    }

    [Fact]
    public async Task Refresh_returns_200_with_rotated_tokens()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "a@org.com", Role = "Viewer" };
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.ValidateAndRotateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((user, "new-refresh", DateTime.UtcNow.AddDays(7)));
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateToken(user)).Returns("new-access");

        var result = await NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object, jwt.Object)
            .Refresh(new RefreshRequest("old-refresh"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<RefreshResponse>>(ok.Value);
        Assert.Equal("new-access", body.Data!.Token);
        Assert.Equal("new-refresh", body.Data!.RefreshToken);
    }

    [Fact]
    public async Task Refresh_returns_401_when_token_invalid()
    {
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.ValidateAndRotateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(((User, string, DateTime)?)null);

        var result = await NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object)
            .Refresh(new RefreshRequest("bad"), default);

        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = Assert.IsType<ApiResponse<object>>(unauth.Value);
        Assert.Equal("INVALID_REFRESH_TOKEN", body.Error!.Code);
    }

    [Fact]
    public async Task Logout_revokes_token_and_returns_200()
    {
        var refresh = new Mock<IRefreshTokenService>();

        var result = await NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object)
            .Logout(new LogoutRequest("some-token"), default);

        Assert.IsType<OkObjectResult>(result);
        refresh.Verify(r => r.RevokeAsync("some-token", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Me_returns_401_when_user_not_found()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(Guid.NewGuid());
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.GetMeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((MeResponse?)null);

        var result = await NewController(auth.Object, currentUser.Object, Mock.Of<ILoginLockoutService>()).Me(default);

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

        var result = await NewController(auth.Object, currentUser.Object, Mock.Of<ILoginLockoutService>()).Me(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<MeResponse>>(ok.Value);
        Assert.Equal("a@org.com", body.Data!.User.Email);
    }
}
