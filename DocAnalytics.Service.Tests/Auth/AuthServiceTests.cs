using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class AuthServiceTests
{
    private static User ActiveUser(string email, string password)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = "Viewer",
            IsActive = true,
            TenantId = Guid.NewGuid(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

    private static Mock<DocAnalytics.Data.AppDbContext> Ctx(User[] users, UserSiteAccess[] access, Site[] sites)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Users).Returns(users.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.UserSiteAccess).Returns(access.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.Sites).Returns(sites.ToList().BuildMockDbSet().Object);
        return ctx;
    }

    [Fact]
    public async Task LoginAsync_returns_null_when_user_not_found()
    {
        var sut = new AuthService(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());
        Assert.Null(await sut.LoginAsync(new LoginRequest("nobody@org.com", "pw"), default));
    }

    [Fact]
    public async Task LoginAsync_returns_null_on_wrong_password()
    {
        var user = ActiveUser("a@org.com", "correct");
        var sut = new AuthService(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());
        Assert.Null(await sut.LoginAsync(new LoginRequest("a@org.com", "wrong"), default));
    }

    [Fact]
    public async Task LoginAsync_returns_token_user_and_sites_on_success()
    {
        var user = ActiveUser("a@org.com", "pw");
        var siteId = Guid.NewGuid();
        var access = new[] { new UserSiteAccess { Id = Guid.NewGuid(), UserId = user.Id, SiteId = siteId } };
        var sites = new[] { new Site { Id = siteId, Name = "Plant One", IsActive = true } };

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("jwt-123");

        var result = await new AuthService(Ctx(new[] { user }, access, sites).Object, jwt.Object)
            .LoginAsync(new LoginRequest("a@org.com", "pw"), default);

        Assert.NotNull(result);
        Assert.Equal("jwt-123", result!.Token);
        Assert.Equal("a@org.com", result.User.Email);
        Assert.Single(result.Sites);
        Assert.Equal("Plant One", result.Sites[0].SiteName);
        jwt.Verify(j => j.CreateToken(It.Is<User>(u => u.Id == user.Id)), Times.Once);
    }

    [Fact]
    public async Task GetMeAsync_returns_null_when_user_missing()
    {
        var sut = new AuthService(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());
        Assert.Null(await sut.GetMeAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task GetMeAsync_returns_user_and_sites()
    {
        var user = ActiveUser("a@org.com", "pw");
        var siteId = Guid.NewGuid();
        var access = new[] { new UserSiteAccess { Id = Guid.NewGuid(), UserId = user.Id, SiteId = siteId } };
        var sites = new[] { new Site { Id = siteId, Name = "Plant One", IsActive = true } };

        var result = await new AuthService(Ctx(new[] { user }, access, sites).Object, Mock.Of<IJwtTokenService>())
            .GetMeAsync(user.Id, default);

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

        var result = await new AuthService(Ctx(Array.Empty<User>(), access, sites).Object, Mock.Of<IJwtTokenService>())
            .GetSitesAsync(userId, default);

        Assert.Single(result);
        Assert.Equal("Active", result[0].SiteName);
    }

    [Fact]
    public async Task ChangePasswordAsync_returns_false_when_user_missing()
    {
        var sut = new AuthService(Ctx(Array.Empty<User>(), Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());
        Assert.False(await sut.ChangePasswordAsync(Guid.NewGuid(), new ChangePasswordRequest("old", "newpassword12"), default));
    }

    [Fact]
    public async Task ChangePasswordAsync_returns_false_on_wrong_current_password()
    {
        var user = ActiveUser("a@org.com", "correct");
        var sut = new AuthService(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());
        Assert.False(await sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("wrong", "newpassword12"), default));
    }

    [Fact]
    public async Task ChangePasswordAsync_updates_hash_and_clears_flag()
    {
        var user = ActiveUser("a@org.com", "oldpassword");
        user.MustChangePassword = true;
        var sut = new AuthService(Ctx(new[] { user }, Array.Empty<UserSiteAccess>(), Array.Empty<Site>()).Object, Mock.Of<IJwtTokenService>());

        var ok = await sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("oldpassword", "newpassword12"), default);

        Assert.True(ok);
        Assert.False(user.MustChangePassword);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword12", user.PasswordHash));
    }

}
