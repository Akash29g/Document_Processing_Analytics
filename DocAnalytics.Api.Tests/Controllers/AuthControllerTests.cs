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
    private static AuthController NewController(
        IAuthService auth, ICurrentUser user, ILoginLockoutService lockout,
        IRefreshTokenService? refresh = null, IJwtTokenService? jwt = null,
        IPasswordResetService? passwordReset = null)
        => new(auth, user, lockout,
               refresh ?? Mock.Of<IRefreshTokenService>(),
               jwt ?? Mock.Of<IJwtTokenService>(),
               passwordReset ?? Mock.Of<IPasswordResetService>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

    [Fact]
    public async Task Login_returns_200_with_envelope_on_success()
    {
        var login = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>(), false);
        var loginResult = new LoginResult(false, null, login);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(loginResult);
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.IssueAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(("raw-refresh", DateTime.UtcNow.AddDays(7)));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object)
            .Login(new LoginRequest("a@org.com", "pw"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<LoginResponse>>(ok.Value);
        Assert.Equal("jwt", body.Data!.Token);
    }

    [Fact]
    public async Task Login_returns_2fa_challenge_when_account_has_2fa_enabled()
    {
        var loginResult = new LoginResult(true, "challenge-token", null);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(loginResult);

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .Login(new LoginRequest("a@org.com", "pw"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<TwoFactorChallengeResponse>>(ok.Value);
        Assert.True(body.Data!.RequiresTwoFactor);
        Assert.Equal("challenge-token", body.Data.ChallengeToken);
    }

    [Fact]
    public async Task Login_returns_401_on_invalid_credentials()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((LoginResult?)null);

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
        auth.Verify(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Login_registers_failure_on_bad_password()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((LoginResult?)null);
        var lockout = new Mock<ILoginLockoutService>();
        lockout.Setup(l => l.IsLockedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((false, 0));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), lockout.Object)
            .Login(new LoginRequest("a@org.com", "bad"), default);

        Assert.IsType<UnauthorizedObjectResult>(result);
        lockout.Verify(l => l.RegisterFailureAsync("a@org.com", It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_sets_refresh_token_cookie_on_success()
    {
        var login = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>(), false);
        var loginResult = new LoginResult(false, null, login);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(loginResult);
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.IssueAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(("raw-refresh", DateTime.UtcNow.AddDays(7)));

        var controller = NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        var result = await controller.Login(new LoginRequest("a@org.com", "pw"), default);

        Assert.IsType<OkObjectResult>(result);
        var setCookie = controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refresh_token=raw-refresh", setCookie);
        Assert.Contains("httponly", setCookie.ToLowerInvariant());
    }

    [Fact]
    public async Task LoginTwoFactor_returns_401_on_invalid_code()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginWithTwoFactorAsync(It.IsAny<TwoFactorLoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResponse?)null);

        var controller = NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>());
        var result = await controller.LoginTwoFactor(new TwoFactorLoginRequest("bad", "000000"), default);

        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("INVALID_2FA_CODE", Assert.IsType<ApiResponse<object>>(unauth.Value).Error!.Code);
    }

    [Fact]
    public async Task LoginTwoFactor_issues_refresh_cookie_on_success()
    {
        var login = new LoginResponse("jwt", new UserDto(Guid.NewGuid(), "a@org.com", "Viewer"), new List<SiteDto>(), false);
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginWithTwoFactorAsync(It.IsAny<TwoFactorLoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.IssueAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(("raw-refresh", DateTime.UtcNow.AddDays(7)));

        var controller = NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        var result = await controller.LoginTwoFactor(new TwoFactorLoginRequest("good", "123456"), default);

        Assert.IsType<OkObjectResult>(result);
        Assert.Contains("refresh_token=raw-refresh", controller.Response.Headers["Set-Cookie"].ToString());
    }

    [Fact]
    public async Task SetupTwoFactor_returns_200_with_setup_payload()
    {
        var payload = new TwoFactorSetupResponse("SECRET", "otpauth://totp/x", "SECR ET");
        var userId = Guid.NewGuid();
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.SetupTwoFactorAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(payload);
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(userId);

        var result = await NewController(auth.Object, currentUser.Object, Mock.Of<ILoginLockoutService>()).SetupTwoFactor(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("SECRET", Assert.IsType<ApiResponse<TwoFactorSetupResponse>>(ok.Value).Data!.Secret);
    }

    [Fact]
    public async Task ConfirmTwoFactor_returns_400_on_error()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.ConfirmTwoFactorAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("Invalid code.", (TwoFactorConfirmResponse?)null));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .ConfirmTwoFactor(new TwoFactorConfirmRequest("000000"), default);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("INVALID_2FA_CODE", Assert.IsType<ApiResponse<object>>(bad.Value).Error!.Code);
    }

    [Fact]
    public async Task ConfirmTwoFactor_returns_200_with_recovery_codes_on_success()
    {
        var payload = new TwoFactorConfirmResponse(new List<string> { "ABCD-1234" });
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.ConfirmTwoFactorAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((string?)null, payload));

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .ConfirmTwoFactor(new TwoFactorConfirmRequest("123456"), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsType<ApiResponse<TwoFactorConfirmResponse>>(ok.Value).Data!.RecoveryCodes);
    }

    [Fact]
    public async Task DisableTwoFactor_returns_400_on_wrong_password()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.DisableTwoFactorAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Password is incorrect.");

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .DisableTwoFactor(new TwoFactorDisableRequest("wrong"), default);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DisableTwoFactor_returns_200_on_success()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.DisableTwoFactorAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var result = await NewController(auth.Object, Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>())
            .DisableTwoFactor(new TwoFactorDisableRequest("correct"), default);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_returns_200_and_rotates_cookie()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "a@org.com", Role = "Viewer" };
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.ValidateAndRotateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((user, "new-refresh", DateTime.UtcNow.AddDays(7)));
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateToken(user)).Returns("new-access");

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object, jwt.Object);
        controller.HttpContext.Request.Headers["Cookie"] = "refresh_token=old-refresh";

        var result = await controller.Refresh(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<RefreshResponse>>(ok.Value);
        Assert.Equal("new-access", body.Data!.Token);

        var setCookie = controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refresh_token=new-refresh", setCookie);
        refresh.Verify(r => r.ValidateAndRotateAsync("old-refresh", It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Refresh_returns_401_when_token_invalid()
    {
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.ValidateAndRotateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(((User, string, DateTime)?)null);

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        controller.HttpContext.Request.Headers["Cookie"] = "refresh_token=bad";

        var result = await controller.Refresh(default);

        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = Assert.IsType<ApiResponse<object>>(unauth.Value);
        Assert.Equal("INVALID_REFRESH_TOKEN", body.Error!.Code);
    }

    [Fact]
    public async Task Logout_revokes_token_and_returns_200()
    {
        var refresh = new Mock<IRefreshTokenService>();

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        controller.HttpContext.Request.Headers["Cookie"] = "refresh_token=some-token";

        var result = await controller.Logout(default);

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
    public async Task Refresh_returns_401_when_cookie_missing()
    {
        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>());
        var result = await controller.Refresh(default);
        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("INVALID_REFRESH_TOKEN", Assert.IsType<ApiResponse<object>>(unauth.Value).Error!.Code);
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

    [Fact]
    public async Task ForgotPassword_returns_200_and_calls_service()
    {
        var reset = new Mock<IPasswordResetService>();
        var controller = NewController(
            Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(),
            null, null, reset.Object);

        var result = await controller.ForgotPassword(new ForgotPasswordRequest("a@org.com"), default);

        Assert.IsType<OkObjectResult>(result);
        reset.Verify(r => r.RequestResetAsync(
            It.IsAny<ForgotPasswordRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_returns_400_on_error()
    {
        var reset = new Mock<IPasswordResetService>();
        reset.Setup(r => r.ResetPasswordAsync(It.IsAny<ResetPasswordRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync("Invalid or expired reset link.");
        var controller = NewController(
            Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(),
            null, null, reset.Object);

        var result = await controller.ResetPassword(new ResetPasswordRequest("bad", "NewPass1!"), default);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("INVALID_RESET", Assert.IsType<ApiResponse<object>>(bad.Value).Error!.Code);
    }

    [Fact]
    public async Task ResetPassword_returns_200_on_success()
    {
        var reset = new Mock<IPasswordResetService>();
        reset.Setup(r => r.ResetPasswordAsync(It.IsAny<ResetPasswordRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((string?)null);
        var controller = NewController(
            Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(),
            null, null, reset.Object);

        var result = await controller.ResetPassword(new ResetPasswordRequest("good", "NewPass1!"), default);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetSessions_returns_200_with_session_list()
    {
        var userId = Guid.NewGuid();
        var sessions = new List<SessionDto> { new(Guid.NewGuid(), "Chrome on Windows", "1.2.3.4", DateTime.UtcNow, DateTime.UtcNow, true) };
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.ListActiveSessionsAsync(userId, It.IsAny<string?>(), It.IsAny<CancellationToken>())).ReturnsAsync(sessions);
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.UserId).Returns(userId);

        var controller = NewController(Mock.Of<IAuthService>(), currentUser.Object, Mock.Of<ILoginLockoutService>(), refresh.Object);
        var result = await controller.GetSessions(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsType<ApiResponse<IReadOnlyList<SessionDto>>>(ok.Value).Data!);
    }

    [Fact]
    public async Task RevokeSession_returns_404_when_not_found_or_not_owned()
    {
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.RevokeSessionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        var result = await controller.RevokeSession(Guid.NewGuid(), default);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RevokeSession_returns_200_on_success()
    {
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.RevokeSessionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        var result = await controller.RevokeSession(Guid.NewGuid(), default);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RevokeOtherSessions_returns_401_when_no_cookie()
    {
        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>());
        var result = await controller.RevokeOtherSessions(default);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task RevokeOtherSessions_returns_200_with_count()
    {
        var refresh = new Mock<IRefreshTokenService>();
        refresh.Setup(r => r.RevokeAllOtherSessionsAsync(It.IsAny<Guid>(), "some-token", It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var controller = NewController(Mock.Of<IAuthService>(), Mock.Of<ICurrentUser>(), Mock.Of<ILoginLockoutService>(), refresh.Object);
        controller.HttpContext.Request.Headers["Cookie"] = "refresh_token=some-token";

        var result = await controller.RevokeOtherSessions(default);

        Assert.IsType<OkObjectResult>(result);
    }
}
