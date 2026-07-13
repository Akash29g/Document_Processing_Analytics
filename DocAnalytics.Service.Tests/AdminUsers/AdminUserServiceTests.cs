using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.AdminUsers;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Provisioning;
using DocAnalytics.Service.Tests.Support;
using Moq;

namespace DocAnalytics.Service.Tests.AdminUsers;

public class AdminUserServiceTests
{
    private readonly TestCurrentUser _me = new() { TenantId = Guid.NewGuid(), SiteId = Guid.NewGuid(), Role = "Admin" };
    private readonly Mock<ICredentialGenerator> _creds = new();
    private readonly Mock<IEmailSender> _email = new();

    public AdminUserServiceTests()
    {
        _creds.Setup(c => c.BuildEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ISet<string>>()))
              .Returns("rita.sharma@acme.com");
        _creds.Setup(c => c.GeneratePassword(It.IsAny<int>())).Returns("TempPass123!");
    }

    private AdminUserService Sut(AppDbContext db) => new(db, _me, _creds.Object, _email.Object);

    private void SeedTenant(AppDbContext db, params Site[] sites)
    {
        db.Tenants.Add(new Tenant { Id = _me.TenantId, Name = "Acme", OrgDomain = "acme.com", IsActive = true, CreatedAt = DateTime.UtcNow });
        db.Sites.AddRange(sites);
        db.SaveChanges();
    }

    private static User Viewer(Guid tenantId, string email) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        Email = email,
        Role = "Viewer",
        IsActive = true,
        PasswordHash = "x",
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task CreateUserAsync_creates_user_and_emails_credentials()
    {
        using var db = InMemoryDb.Create(_me);
        var siteId = Guid.NewGuid();
        SeedTenant(db, new Site { Id = siteId, TenantId = _me.TenantId, Name = "Plant", IsActive = true, CreatedAt = DateTime.UtcNow });

        var result = await Sut(db).CreateUserAsync(
            new AdminCreateUserRequest("Rita", "Sharma", new List<Guid> { siteId }), default);

        Assert.NotNull(result);
        Assert.Equal("rita.sharma@acme.com", result!.Email);
        Assert.True(result.CredentialsEmailed);
        _email.Verify(e => e.SendAsync("rita.sharma@acme.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(1, db.Users.Count());
        Assert.Equal(1, db.UserSiteAccess.Count());
    }

    [Fact]
    public async Task CreateUserAsync_returns_null_for_invalid_site()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenant(db);   // no sites at all
        var result = await Sut(db).CreateUserAsync(
            new AdminCreateUserRequest("Rita", "Sharma", new List<Guid> { Guid.NewGuid() }), default);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersAsync_returns_only_viewers_in_my_tenant()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenant(db);
        db.Users.AddRange(
            Viewer(_me.TenantId, "v@acme.com"),
            new User { Id = Guid.NewGuid(), TenantId = _me.TenantId, Email = "admin@acme.com", Role = "Admin", IsActive = true, PasswordHash = "x", CreatedAt = DateTime.UtcNow },
            Viewer(Guid.NewGuid(), "other@x.com"));
        db.SaveChanges();

        var users = await Sut(db).GetUsersAsync(default);
        Assert.Single(users);
        Assert.Equal("v@acme.com", users[0].Email);
    }

    [Fact]
    public async Task DeactivateUserAsync_soft_deletes_viewer()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenant(db);
        var u = Viewer(_me.TenantId, "v@acme.com");
        db.Users.Add(u);
        db.SaveChanges();

        Assert.True(await Sut(db).DeactivateUserAsync(u.Id, default));
        Assert.False(db.Users.Single(x => x.Id == u.Id).IsActive);
    }

    [Fact]
    public async Task DeactivateUserAsync_returns_false_when_missing()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenant(db);
        Assert.False(await Sut(db).DeactivateUserAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task CreateSiteAsync_adds_site_and_grants_creator_access()
    {
        using var db = InMemoryDb.Create(_me);
        SeedTenant(db);
        var dto = await Sut(db).CreateSiteAsync(new AdminCreateSiteRequest("New Plant", "Pune"), default);
        Assert.Equal("New Plant", dto.Name);
        Assert.Equal(1, db.Sites.Count());
        Assert.Equal(1, db.UserSiteAccess.Count(a => a.UserId == _me.UserId));
    }
}
