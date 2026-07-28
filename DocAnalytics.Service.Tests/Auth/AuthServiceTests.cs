using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using DocAnalytics.Service.Tests.Support;
using Microsoft.AspNetCore.DataProtection;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class AuthServiceTests
{
    // Ephemeral (in-memory, non-persistent) protector — perfect for unit tests, no Postgres key ring needed.
    private static readonly IDataProtectionProvider DataProtection = new EphemeralDataProtectionProvider();

    private static User ActiveUser(string email, string password, bool twoFactorEnabled = false, string? twoFactorSecret = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = "Viewer",
            IsActive = true,
            TenantId = Guid.NewGuid(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            TwoFactorEnabled = twoFactorEnabled,
            TwoFactorSecret = twoFactorSecret,
        };

    private static Mock<DocAnalytics.Data.AppDbContext> Ctx(
        User[] users, UserSiteAccess[] access, Site[] sites, TwoFactorRecoveryCode[]? recoveryCodes = null)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Users).Returns(users.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.UserSiteAccess).Returns(access.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.Sites).Returns(sites.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.TwoFactorRecoveryCodes).Returns((recoveryCodes ?? Array.Empty<TwoFactorRecoveryCode>()).ToList().BuildMockDbSet().Object);
        return ctx;
    }

    private static AuthService NewSut(
        Mock<DocAnalytics.Data.AppDbContext> ctx, IJwtTokenService? jwt = null, ITwoFactorService? twoFactor = null)
        => new(ctx.Object, jwt ?? Mock.Of<IJwtTokenService>(), Mock.Of<IPasswordPolicy>(),
               twoFactor ?? new TwoFactorService(), DataProtection);

    [Fact]
    public async Task LoginAsync_returns_null_when_user_not_found()
    {
        var sut = NewSut(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        Assert.Null(await sut.LoginAsync(new LoginRequest("nobody@org.com", "pw"), default));
    }

    [Fact]
    public async Task LoginAsync_returns_null_on_wrong_password()
    {
        var user = ActiveUser("a@org.com", "correct");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        Assert.Null(await sut.LoginAsync(new LoginRequest("a@org.com", "wrong"), default));
    }

    [Fact]
    public async Task LoginAsync_returns_full_login_when_2fa_disabled()
    {
        var user = ActiveUser("a@org.com", "pw");
        var siteId = Guid.NewGuid();
        var access = new[] { new UserSiteAccess { Id = Guid.NewGuid(), UserId = user.Id, SiteId = siteId } };
        var sites = new[] { new Site { Id = siteId, Name = "Plant One", IsActive = true } };

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("jwt-123");

        var result = await NewSut(Ctx(new[] { user }, access, sites), jwt.Object)
            .LoginAsync(new LoginRequest("a@org.com", "pw"), default);

        Assert.NotNull(result);
        Assert.False(result!.RequiresTwoFactor);
        Assert.NotNull(result.Login);
        Assert.Equal("jwt-123", result.Login!.Token);
        Assert.Equal("a@org.com", result.Login.User.Email);
        Assert.Single(result.Login.Sites);
        Assert.Equal("Plant One", result.Login.Sites[0].SiteName);
        jwt.Verify(j => j.CreateToken(It.Is<User>(u => u.Id == user.Id)), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_returns_challenge_when_2fa_enabled_and_never_issues_a_real_token()
    {
        var user = ActiveUser("a@org.com", "pw", twoFactorEnabled: true, twoFactorSecret: "irrelevant-for-this-test");
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateTwoFactorChallengeToken(user.Id)).Returns("challenge-abc");

        var result = await NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()), jwt.Object)
            .LoginAsync(new LoginRequest("a@org.com", "pw"), default);

        Assert.NotNull(result);
        Assert.True(result!.RequiresTwoFactor);
        Assert.Equal("challenge-abc", result.ChallengeToken);
        Assert.Null(result.Login);
        jwt.Verify(j => j.CreateToken(It.IsAny<User>()), Times.Never); // never issue the real token pre-2FA
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_returns_null_on_invalid_challenge_token()
    {
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.ValidateTwoFactorChallengeToken("bad")).Returns((Guid?)null);

        var sut = NewSut(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()), jwt.Object);
        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("bad", "123456"), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_succeeds_with_a_valid_totp_code()
    {
        var twoFactor = new TwoFactorService();
        var (secret, _, _) = twoFactor.GenerateSetup("a@org.com");

        var dp = DataProtection.CreateProtector("DocAnalytics.TwoFactorSecret");
        var user = ActiveUser("a@org.com", "pw", twoFactorEnabled: true, twoFactorSecret: dp.Protect(secret));

        var validCode = new OtpNet.Totp(OtpNet.Base32Encoding.ToBytes(secret)).ComputeTotp();

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.ValidateTwoFactorChallengeToken("good-challenge")).Returns(user.Id);
        jwt.Setup(j => j.CreateToken(user)).Returns("jwt-final");

        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()), jwt.Object, twoFactor);
        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("good-challenge", validCode), default);

        Assert.NotNull(result);
        Assert.Equal("jwt-final", result!.Token);
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_falls_back_to_a_valid_recovery_code()
    {
        var twoFactor = new TwoFactorService();
        var dp = DataProtection.CreateProtector("DocAnalytics.TwoFactorSecret");
        var user = ActiveUser("a@org.com", "pw", twoFactorEnabled: true, twoFactorSecret: dp.Protect("ANYSECRETXXXX"));

        var recoveryPlain = "ABCD-1234";
        var recoveryCode = new TwoFactorRecoveryCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = twoFactor.HashRecoveryCode(recoveryPlain),
        };

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.ValidateTwoFactorChallengeToken("good-challenge")).Returns(user.Id);
        jwt.Setup(j => j.CreateToken(user)).Returns("jwt-final");

        var sut = NewSut(
            Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>(), new[] { recoveryCode }),
            jwt.Object, twoFactor);

        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("good-challenge", recoveryPlain), default);

        Assert.NotNull(result);
        Assert.Equal("jwt-final", result!.Token);
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_rejects_a_wrong_code_and_a_wrong_recovery_code()
    {
        var twoFactor = new TwoFactorService();
        var dp = DataProtection.CreateProtector("DocAnalytics.TwoFactorSecret");
        var (secret, _, _) = twoFactor.GenerateSetup("a@org.com");
        var user = ActiveUser("a@org.com", "pw", twoFactorEnabled: true, twoFactorSecret: dp.Protect(secret));

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.ValidateTwoFactorChallengeToken("good-challenge")).Returns(user.Id);

        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()), jwt.Object, twoFactor);
        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("good-challenge", "000000"), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetupTwoFactorAsync_stores_an_encrypted_secret_and_returns_setup_payload()
    {
        var user = ActiveUser("a@org.com", "pw");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));

        var result = await sut.SetupTwoFactorAsync(user.Id, default);

        Assert.False(string.IsNullOrWhiteSpace(result.Secret));
        Assert.StartsWith("otpauth://totp/", result.OtpAuthUri);
        Assert.NotNull(user.TwoFactorSecret);
        Assert.NotEqual(result.Secret, user.TwoFactorSecret); // stored value is encrypted, not plaintext
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_enables_2fa_and_returns_recovery_codes_on_valid_code()
    {
        var twoFactor = new TwoFactorService();
        var user = ActiveUser("a@org.com", "pw");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()), twoFactor: twoFactor);

        var setup = await sut.SetupTwoFactorAsync(user.Id, default);
        var code = new OtpNet.Totp(OtpNet.Base32Encoding.ToBytes(setup.Secret)).ComputeTotp();

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, code, default);

        Assert.Null(error);
        Assert.NotNull(result);
        Assert.Equal(10, result!.RecoveryCodes.Count);
        Assert.True(user.TwoFactorEnabled);
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_returns_error_on_invalid_code_and_does_not_enable_2fa()
    {
        var user = ActiveUser("a@org.com", "pw");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        await sut.SetupTwoFactorAsync(user.Id, default);

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, "000000", default);

        Assert.NotNull(error);
        Assert.Null(result);
        Assert.False(user.TwoFactorEnabled);
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_returns_error_when_setup_was_never_started()
    {
        var user = ActiveUser("a@org.com", "pw");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, "123456", default);

        Assert.NotNull(error);
        Assert.Null(result);
    }

    [Fact]
    public async Task DisableTwoFactorAsync_returns_error_on_wrong_password()
    {
        var user = ActiveUser("a@org.com", "correct", twoFactorEnabled: true);
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));

        var error = await sut.DisableTwoFactorAsync(user.Id, "wrong", default);

        Assert.NotNull(error);
        Assert.True(user.TwoFactorEnabled);
    }

    [Fact]
    public async Task DisableTwoFactorAsync_clears_2fa_on_correct_password()
    {
        var user = ActiveUser("a@org.com", "correct", twoFactorEnabled: true, twoFactorSecret: "enc-secret");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));

        var error = await sut.DisableTwoFactorAsync(user.Id, "correct", default);

        Assert.Null(error);
        Assert.False(user.TwoFactorEnabled);
        Assert.Null(user.TwoFactorSecret);
    }

    [Fact]
    public async Task GetMeAsync_returns_null_when_user_missing()
    {
        var sut = NewSut(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        Assert.Null(await sut.GetMeAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task GetMeAsync_returns_user_and_sites()
    {
        var user = ActiveUser("a@org.com", "pw");
        var siteId = Guid.NewGuid();
        var access = new[] { new UserSiteAccess { Id = Guid.NewGuid(), UserId = user.Id, SiteId = siteId } };
        var sites = new[] { new Site { Id = siteId, Name = "Plant One", IsActive = true } };

        var result = await NewSut(Ctx(new[] { user }, access, sites)).GetMeAsync(user.Id, default);

        Assert.NotNull(result);
        Assert.Equal("a@org.com", result!.User.Email);
        Assert.Single(result.Sites);
    }

    [Fact]
    public async Task GetSitesAsync_excludes_inactive_sites()
    {
        var userId = Guid.NewGuid();
        var activeId = Guid.NewGuid(); var inactiveId = Guid.NewGuid();
        var access = new[]
        {
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = userId, SiteId = activeId },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = userId, SiteId = inactiveId },
        };
        var sites = new[]
        {
            new Site { Id = activeId, Name = "Active", IsActive = true },
            new Site { Id = inactiveId, Name = "Inactive", IsActive = false },
        };

        var result = await NewSut(Ctx(Array.Empty<User>(), access, sites)).GetSitesAsync(userId, default);

        Assert.Single(result);
        Assert.Equal("Active", result[0].SiteName);
    }

    [Fact]
    public async Task ChangePasswordAsync_returns_error_when_user_missing()
    {
        var sut = NewSut(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        Assert.NotNull(await sut.ChangePasswordAsync(Guid.NewGuid(), new ChangePasswordRequest("old", "newpassword12"), default));
    }

    [Fact]
    public async Task ChangePasswordAsync_returns_error_on_wrong_current_password()
    {
        var user = ActiveUser("a@org.com", "correct");
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));
        Assert.NotNull(await sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("wrong", "newpassword12"), default));
    }

    [Fact]
    public async Task ChangePasswordAsync_updates_hash_and_clears_flag()
    {
        var user = ActiveUser("a@org.com", "oldpassword");
        user.MustChangePassword = true;
        var sut = NewSut(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()));

        var error = await sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("oldpassword", "newpassword12"), default);
        Assert.Null(error);
        Assert.False(user.MustChangePassword);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword12", user.PasswordHash));
    }
}
