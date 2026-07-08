using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using DocAnalytics.Tests.Support;
using Microsoft.Extensions.Configuration;

namespace DocAnalytics.Tests.Auth;

public class AuthServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();
    private readonly Guid _site = Guid.NewGuid();

    private AppDbContext NewDb() =>
        TestDb.Create(new FakeCurrentUser { TenantId = _tenant, SiteId = _site });

    private static IJwtTokenService NewJwt()
    {
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "super-secret-test-key-that-is-long-enough-1234567890",
            ["Jwt:Issuer"] = "DocAnalytics",
            ["Jwt:Audience"] = "DocAnalyticsClient",
            ["Jwt:ExpiryMinutes"] = "120",
        }).Build();
        return new JwtTokenService(cfg);
    }

    // Seeds one active user (+ optional site access) and returns the plaintext password.
    private (User user, string password) SeedUser(AppDbContext db, string email = "a@org.com",
        bool active = true, bool grantSite = true)
    {
        const string pwd = "P@ssw0rd!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(pwd),
            Role = "Viewer",
            IsActive = active,
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        if (grantSite)
        {
            db.Sites.Add(new Site { Id = _site, TenantId = _tenant, Name = "Mumbai Plant", IsActive = true, CreatedAt = DateTime.UtcNow });
            db.Set<UserSiteAccess>().Add(new UserSiteAccess { Id = Guid.NewGuid(), UserId = user.Id, SiteId = _site, GrantedAt = DateTime.UtcNow });
        }
        return (user, pwd);
    }

    [Fact]
    public async Task Login_valid_credentials_returns_token_and_sites()
    {
        using var db = NewDb();
        var (user, pwd) = SeedUser(db);
        await db.SaveChangesAsync();

        var svc = new AuthService(db, NewJwt());
        var res = await svc.LoginAsync(new LoginRequest(user.Email, pwd), default);

        Assert.NotNull(res);
        Assert.False(string.IsNullOrWhiteSpace(res!.Token));
        Assert.Equal(user.Email, res.User.Email);
        Assert.Single(res.Sites);
    }

    [Fact]
    public async Task Login_wrong_password_returns_null()   // controller → 401
    {
        using var db = NewDb();
        var (user, _) = SeedUser(db);
        await db.SaveChangesAsync();

        var res = await new AuthService(db, NewJwt())
            .LoginAsync(new LoginRequest(user.Email, "wrong"), default);

        Assert.Null(res);
    }

    [Fact]
    public async Task Login_unknown_email_returns_null()
    {
        using var db = NewDb();
        var res = await new AuthService(db, NewJwt())
            .LoginAsync(new LoginRequest("nobody@org.com", "x"), default);
        Assert.Null(res);
    }

    [Fact]
    public async Task Login_inactive_user_returns_null()
    {
        using var db = NewDb();
        var (user, pwd) = SeedUser(db, active: false);
        await db.SaveChangesAsync();
        var res = await new AuthService(db, NewJwt())
            .LoginAsync(new LoginRequest(user.Email, pwd), default);
        Assert.Null(res);
    }

    [Fact]
    public async Task GetMe_returns_profile_and_sites()
    {
        using var db = NewDb();
        var (user, _) = SeedUser(db);
        await db.SaveChangesAsync();

        var me = await new AuthService(db, NewJwt()).GetMeAsync(user.Id, default);

        Assert.NotNull(me);
        Assert.Equal(user.Email, me!.User.Email);
        Assert.Single(me.Sites);
    }

    [Fact]
    public void Jwt_token_is_issued_non_empty()
    {
        var user = new User { Id = Guid.NewGuid(), TenantId = _tenant, Email = "a@org.com", Role = "Admin" };
        var token = NewJwt().CreateToken(user);
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains(".", token);   // JWT has header.payload.signature
    }
}
