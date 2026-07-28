using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class RefreshTokenServiceTests
{
    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options,
            Mock.Of<ICurrentUser>());


    private static IConfiguration NewConfig(int? refreshDays = null)
    {
        var dict = new Dictionary<string, string?>();
        if (refreshDays is not null) dict["Jwt:RefreshExpiryDays"] = refreshDays.ToString();
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    // NOTE: adjust required fields here if your User entity needs more (e.g. TenantId, CreatedAt).
    private static User NewActiveUser(AppDbContext db)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = $"{Guid.NewGuid()}@test.com",
            PasswordHash = "irrelevant",
            Role = "Admin",
            IsActive = true,
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    private static string HashForTest(string raw)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public async Task IssueAsync_CreatesToken_WithDeviceInfo_AndDefaultSevenDayExpiry()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);

        var (raw, expiresAt) = await sut.IssueAsync(user.Id, "1.2.3.4", "TestAgent/1.0", CancellationToken.None);

        Assert.NotEmpty(raw);
        var stored = await db.RefreshTokens.SingleAsync();
        Assert.Equal(user.Id, stored.UserId);
        Assert.Equal("1.2.3.4", stored.IpAddress);
        Assert.Equal("TestAgent/1.0", stored.UserAgent);
        Assert.Null(stored.RevokedAt);
        Assert.True((expiresAt - DateTime.UtcNow).TotalDays is > 6.9 and < 7.1);
    }

    [Fact]
    public async Task IssueAsync_RespectsConfiguredLifetimeDays()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig(30));
        var user = NewActiveUser(db);

        var (_, expiresAt) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);

        Assert.True((expiresAt - DateTime.UtcNow).TotalDays is > 29.9 and < 30.1);
    }

    [Fact]
    public async Task ValidateAndRotateAsync_ReturnsNull_ForUnknownToken()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());

        Assert.Null(await sut.ValidateAndRotateAsync("not-a-real-token", "1.2.3.4", "Agent", CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAndRotateAsync_ReturnsNull_ForEmptyOrNullToken()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());

        Assert.Null(await sut.ValidateAndRotateAsync("", "1.2.3.4", null, CancellationToken.None));
        Assert.Null(await sut.ValidateAndRotateAsync(null!, "1.2.3.4", null, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAndRotateAsync_RotatesToken_OnValidPresentation()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var (raw, _) = await sut.IssueAsync(user.Id, "1.2.3.4", "AgentA", CancellationToken.None);

        var result = await sut.ValidateAndRotateAsync(raw, "5.6.7.8", "AgentB", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Value.User.Id);
        Assert.NotEqual(raw, result.Value.RawToken);

        var allTokens = await db.RefreshTokens.ToListAsync();
        Assert.Equal(2, allTokens.Count);
        var oldToken = allTokens.Single(t => t.RevokedAt != null);
        var newToken = allTokens.Single(t => t.RevokedAt == null);
        Assert.Equal(oldToken.ReplacedByTokenHash, newToken.TokenHash);
        Assert.Equal("5.6.7.8", newToken.IpAddress);
        Assert.Equal("AgentB", newToken.UserAgent);
    }

    [Fact]
    public async Task ValidateAndRotateAsync_ReturnsNull_ForExpiredToken()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);

        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashForTest("expired-token"),
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddDays(-3),
        });
        await db.SaveChangesAsync();

        Assert.Null(await sut.ValidateAndRotateAsync("expired-token", null, null, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAndRotateAsync_ReturnsNull_WhenUserDeactivated()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var (raw, _) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);

        user.IsActive = false;
        await db.SaveChangesAsync();

        Assert.Null(await sut.ValidateAndRotateAsync(raw, null, null, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAndRotateAsync_DetectsReuse_AndRevokesAllTokensForUser()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);

        var (raw, _) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        var first = await sut.ValidateAndRotateAsync(raw, null, null, CancellationToken.None);
        Assert.NotNull(first);

        var second = await sut.ValidateAndRotateAsync(raw, null, null, CancellationToken.None);
        Assert.Null(second);

        var allTokens = await db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync();
        Assert.All(allTokens, t => Assert.NotNull(t.RevokedAt));
    }

    [Fact]
    public async Task RevokeAsync_RevokesMatchingToken()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var (raw, _) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);

        await sut.RevokeAsync(raw, CancellationToken.None);

        Assert.NotNull((await db.RefreshTokens.SingleAsync()).RevokedAt);
    }

    [Fact]
    public async Task RevokeAsync_IsNoOp_ForUnknownOrEmptyToken()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());

        await sut.RevokeAsync("does-not-exist", CancellationToken.None);
        await sut.RevokeAsync("", CancellationToken.None);

        Assert.Empty(await db.RefreshTokens.ToListAsync());
    }

    [Fact]
    public async Task RevokeAllForUserAsync_RevokesEveryActiveToken_ForThatUserOnly()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var otherUser = NewActiveUser(db);

        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        await sut.IssueAsync(otherUser.Id, null, null, CancellationToken.None);

        await sut.RevokeAllForUserAsync(user.Id, CancellationToken.None);

        var userTokens = await db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync();
        Assert.All(userTokens, t => Assert.NotNull(t.RevokedAt));

        var otherTokens = await db.RefreshTokens.Where(t => t.UserId == otherUser.Id).ToListAsync();
        Assert.All(otherTokens, t => Assert.Null(t.RevokedAt));
    }

    [Fact]
    public async Task ListActiveSessionsAsync_ReturnsOnlyActiveUnexpired_MarksCurrentCorrectly()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);

        var (currentRaw, _) = await sut.IssueAsync(user.Id, "1.1.1.1", "Chrome/1", CancellationToken.None);
        await sut.IssueAsync(user.Id, "2.2.2.2", "Firefox/1", CancellationToken.None);
        var (revokedRaw, _) = await sut.IssueAsync(user.Id, "3.3.3.3", "Safari/1", CancellationToken.None);
        await sut.RevokeAsync(revokedRaw, CancellationToken.None);

        var sessions = await sut.ListActiveSessionsAsync(user.Id, currentRaw, CancellationToken.None);

        Assert.Equal(2, sessions.Count);
        Assert.Contains(sessions, s => s.IsCurrent);
        Assert.DoesNotContain(sessions, s => s.IpAddress == "3.3.3.3");
    }

    [Fact]
    public async Task ListActiveSessionsAsync_MarksNoneCurrent_WhenNoCurrentTokenProvided()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);

        var sessions = await sut.ListActiveSessionsAsync(user.Id, null, CancellationToken.None);

        Assert.All(sessions, s => Assert.False(s.IsCurrent));
    }

    [Fact]
    public async Task RevokeSessionAsync_RevokesOwnSession_ReturnsTrue()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        var tokenId = (await db.RefreshTokens.SingleAsync()).Id;

        Assert.True(await sut.RevokeSessionAsync(user.Id, tokenId, CancellationToken.None));
        Assert.NotNull((await db.RefreshTokens.SingleAsync()).RevokedAt);
    }

    [Fact]
    public async Task RevokeSessionAsync_ReturnsFalse_WhenSessionBelongsToAnotherUser()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var owner = NewActiveUser(db);
        var attacker = NewActiveUser(db);
        await sut.IssueAsync(owner.Id, null, null, CancellationToken.None);
        var tokenId = (await db.RefreshTokens.SingleAsync()).Id;

        Assert.False(await sut.RevokeSessionAsync(attacker.Id, tokenId, CancellationToken.None));
        Assert.Null((await db.RefreshTokens.SingleAsync()).RevokedAt);
    }

    [Fact]
    public async Task RevokeSessionAsync_ReturnsFalse_ForAlreadyRevokedSession()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var (raw, _) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        await sut.RevokeAsync(raw, CancellationToken.None);
        var tokenId = (await db.RefreshTokens.SingleAsync()).Id;

        Assert.False(await sut.RevokeSessionAsync(user.Id, tokenId, CancellationToken.None));
    }

    [Fact]
    public async Task RevokeAllOtherSessionsAsync_RevokesEverythingExceptCurrent()
    {
        using var db = NewDb();
        var sut = new RefreshTokenService(db, NewConfig());
        var user = NewActiveUser(db);
        var (currentRaw, _) = await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);
        await sut.IssueAsync(user.Id, null, null, CancellationToken.None);

        var revokedCount = await sut.RevokeAllOtherSessionsAsync(user.Id, currentRaw, CancellationToken.None);

        Assert.Equal(2, revokedCount);
        var sessions = await sut.ListActiveSessionsAsync(user.Id, currentRaw, CancellationToken.None);
        Assert.Single(sessions);
        Assert.True(sessions[0].IsCurrent);
    }
}
