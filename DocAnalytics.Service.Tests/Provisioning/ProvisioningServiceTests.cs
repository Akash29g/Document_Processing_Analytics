using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Provisioning;
using DocAnalytics.Service.Tests.Support;
using Moq;

namespace DocAnalytics.Service.Tests.Provisioning;

public class ProvisioningServiceTests
{
	private readonly Mock<ICredentialGenerator> _creds = new();
	private readonly Mock<IEmailSender> _email = new();
	private readonly TestCurrentUser _dev = new() { Role = "Developer" };

	public ProvisioningServiceTests()
	{
		_creds.Setup(c => c.BuildEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ISet<string>>()))
			  .Returns("admin@initech.com");
		_creds.Setup(c => c.GeneratePassword(It.IsAny<int>())).Returns("TempPass123!");
	}

	private ProvisioningService Sut(AppDbContext db) => new(db, _creds.Object, _email.Object);

	private static Tenant Tenant(Guid id, string domain = "initech.com", bool active = true) =>
		new() { Id = id, Name = "Initech", OrgDomain = domain, IsActive = active, CreatedAt = DateTime.UtcNow };

	private static User User(Guid tenantId, string email, string role) => new()
	{
		Id = Guid.NewGuid(),
		TenantId = tenantId,
		Email = email,
		Role = role,
		IsActive = true,
		PasswordHash = "x",
		CreatedAt = DateTime.UtcNow
	};

	[Fact]
	public async Task CreateTenantAsync_creates_tenant()
	{
		using var db = InMemoryDb.Create(_dev);
		var dto = await Sut(db).CreateTenantAsync(new CreateTenantRequest("Initech", "initech.com"), default);
		Assert.NotNull(dto);
		Assert.Equal("initech.com", dto!.OrgDomain);
		Assert.Equal(1, db.Tenants.Count());
	}

	[Fact]
	public async Task CreateTenantAsync_returns_null_when_domain_taken()
	{
		using var db = InMemoryDb.Create(_dev);
		db.Tenants.Add(Tenant(Guid.NewGuid()));
		db.SaveChanges();
		Assert.Null(await Sut(db).CreateTenantAsync(new CreateTenantRequest("Dup", "initech.com"), default));
	}

	[Fact]
	public async Task GetTenantsAsync_returns_counts()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		db.Tenants.Add(Tenant(tid));
		db.Sites.Add(new Site { Id = Guid.NewGuid(), TenantId = tid, Name = "S1", IsActive = true, CreatedAt = DateTime.UtcNow });
		db.Users.Add(User(tid, "a@initech.com", "Admin"));
		db.SaveChanges();

		var list = await Sut(db).GetTenantsAsync(default);
		Assert.Single(list);
		Assert.Equal(1, list[0].SiteCount);
		Assert.Equal(1, list[0].AdminCount);
	}

	[Fact]
	public async Task CreateAdminAsync_creates_admin_grants_all_sites_and_emails()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		db.Tenants.Add(Tenant(tid));
		db.Sites.AddRange(
			new Site { Id = Guid.NewGuid(), TenantId = tid, Name = "S1", IsActive = true, CreatedAt = DateTime.UtcNow },
			new Site { Id = Guid.NewGuid(), TenantId = tid, Name = "S2", IsActive = true, CreatedAt = DateTime.UtcNow });
		db.SaveChanges();

		var dto = await Sut(db).CreateAdminAsync(tid, new CreateAdminRequest("Bill", "Lumbergh"), _dev.UserId, default);

		Assert.NotNull(dto);
		Assert.Equal("Admin", dto!.Role);
		_email.Verify(e => e.SendAsync("admin@initech.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
		Assert.Equal(2, db.UserSiteAccess.Count());
	}

	[Fact]
	public async Task CreateAdminAsync_returns_null_when_tenant_missing()
	{
		using var db = InMemoryDb.Create(_dev);
		Assert.Null(await Sut(db).CreateAdminAsync(Guid.NewGuid(), new CreateAdminRequest("A", "B"), _dev.UserId, default));
	}

	[Fact]
	public async Task RemoveAdminAsync_deactivates_admin()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		var u = User(tid, "a@initech.com", "Admin");
		db.Tenants.Add(Tenant(tid)); db.Users.Add(u); db.SaveChanges();

		Assert.True(await Sut(db).RemoveAdminAsync(tid, u.Id, default));
		Assert.False(db.Users.Single(x => x.Id == u.Id).IsActive);
	}

	[Fact]
	public async Task RemoveAdminAsync_returns_false_for_viewer()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		var u = User(tid, "v@initech.com", "Viewer");
		db.Tenants.Add(Tenant(tid)); db.Users.Add(u); db.SaveChanges();

		Assert.False(await Sut(db).RemoveAdminAsync(tid, u.Id, default));   // role mismatch
	}

	[Fact]
	public async Task CreateSiteAsync_adds_site()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		db.Tenants.Add(Tenant(tid)); db.SaveChanges();

		var dto = await Sut(db).CreateSiteAsync(tid, new CreateSiteRequest("Plant", "Austin"), default);
		Assert.NotNull(dto);
		Assert.Equal(1, db.Sites.Count());
	}

	[Fact]
	public async Task CreateSiteAsync_returns_null_when_tenant_missing()
	{
		using var db = InMemoryDb.Create(_dev);
		Assert.Null(await Sut(db).CreateSiteAsync(Guid.NewGuid(), new CreateSiteRequest("Plant", null), default));
	}

	[Fact]
	public async Task RemoveSiteAsync_soft_deletes_and_revokes_access()
	{
		using var db = InMemoryDb.Create(_dev);
		var tid = Guid.NewGuid();
		var sid = Guid.NewGuid();
		db.Tenants.Add(Tenant(tid));
		db.Sites.Add(new Site { Id = sid, TenantId = tid, Name = "S", IsActive = true, CreatedAt = DateTime.UtcNow });
		db.UserSiteAccess.Add(new UserSiteAccess { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), SiteId = sid, GrantedAt = DateTime.UtcNow });
		db.SaveChanges();

		Assert.True(await Sut(db).RemoveSiteAsync(tid, sid, default));
		Assert.False(db.Sites.Single(s => s.Id == sid).IsActive);
		Assert.Empty(db.UserSiteAccess.Where(a => a.SiteId == sid));
	}
}
