using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Tests.Support;
using Microsoft.EntityFrameworkCore;


namespace DocAnalytics.Service.Tests.Alerts;

public class AlertRuleServiceTests
{
    private readonly TestCurrentUser _admin = new() { TenantId = Guid.NewGuid(), SiteId = Guid.NewGuid(), Role = "Admin" };

    private AlertRuleService Sut(AppDbContext db, ICurrentUser? me = null) => new(db, me ?? _admin);

    private static CreateAlertRuleRequest NewReq(string name = "High failure") => new()
    {
        Name = name,
        ThresholdPercent = 10,
        WindowMinutes = 60,
        Email = "ops@acme.com",
        CooldownMinutes = 60,
        IsEnabled = true
    };

    [Fact]
    public async Task CreateAsync_stamps_tenant_and_site_and_persists()
    {
        using var db = InMemoryDb.Create(_admin);
        var dto = await Sut(db).CreateAsync(NewReq(), default);

        Assert.Equal("High failure", dto.Name);
        var saved = db.AlertRules.Single();          // visible: stamped with _admin's tenant/site
        Assert.Equal(_admin.TenantId, saved.TenantId);
        Assert.Equal(_admin.SiteId, saved.SiteId);
    }

    [Fact]
    public async Task ListAsync_returns_all_rules_for_admin()
    {
        using var db = InMemoryDb.Create(_admin);
        var sut = Sut(db);
        await sut.CreateAsync(NewReq("A"), default);
        await sut.CreateAsync(NewReq("B"), default);

        Assert.Equal(2, (await sut.ListAsync(default)).Count);
    }

    [Fact]
    public async Task UpdateAsync_changes_fields()
    {
        using var db = InMemoryDb.Create(_admin);
        var sut = Sut(db);
        var created = await sut.CreateAsync(NewReq(), default);

        var updated = await sut.UpdateAsync(created.Id, new UpdateAlertRuleRequest
        {
            Name = "Renamed",
            ThresholdPercent = 25,
            WindowMinutes = 30,
            Email = "new@acme.com",
            CooldownMinutes = 15,
            IsEnabled = false
        }, default);

        Assert.NotNull(updated);
        Assert.Equal("Renamed", updated!.Name);
        Assert.Equal(25, updated.ThresholdPercent);
        Assert.False(updated.IsEnabled);
    }

    [Fact]
    public async Task UpdateAsync_returns_null_when_missing()
    {
        using var db = InMemoryDb.Create(_admin);
        Assert.Null(await Sut(db).UpdateAsync(Guid.NewGuid(),
            new UpdateAlertRuleRequest { Name = "x", Email = "x@x.com" }, default));
    }

    [Fact]
    public async Task DeleteAsync_removes_rule()
    {
        using var db = InMemoryDb.Create(_admin);
        var sut = Sut(db);
        var created = await sut.CreateAsync(NewReq(), default);
        Assert.True(await sut.DeleteAsync(created.Id, default));
        Assert.Empty(db.AlertRules);
    }

    [Fact]
    public async Task DeleteAsync_returns_false_when_missing()
    {
        using var db = InMemoryDb.Create(_admin);
        Assert.False(await Sut(db).DeleteAsync(Guid.NewGuid(), default));
    }
    [Fact]
    public async Task GetAsync_returns_null_when_missing()
    {
        using var db = InMemoryDb.Create(_admin);
        Assert.Null(await Sut(db).GetAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task GetAsync_returns_null_for_viewer_not_recipient()
    {
        var viewer = new TestCurrentUser { TenantId = _admin.TenantId, SiteId = _admin.SiteId, Role = "Viewer" };
        using var db = InMemoryDb.Create(viewer);
        db.Users.Add(new User { Id = viewer.UserId, TenantId = viewer.TenantId, Email = "someone@acme.com", Role = "Viewer", IsActive = true, PasswordHash = "x", CreatedAt = DateTime.UtcNow });
        db.AlertRules.Add(new AlertRule { Id = Guid.NewGuid(), TenantId = viewer.TenantId, SiteId = viewer.SiteId, Name = "R", ThresholdPercent = 10, WindowMinutes = 60, Email = "ops@acme.com", CooldownMinutes = 60, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        db.SaveChanges();
        var id = db.AlertRules.IgnoreQueryFilters().Single().Id;

        Assert.Null(await new AlertRuleService(db, viewer).GetAsync(id, default));
    }

    [Fact]
    public async Task GetAsync_returns_rule_for_recipient_viewer()
    {
        var viewer = new TestCurrentUser { TenantId = _admin.TenantId, SiteId = _admin.SiteId, Role = "Viewer" };
        using var db = InMemoryDb.Create(viewer);
        db.Users.Add(new User { Id = viewer.UserId, TenantId = viewer.TenantId, Email = "viewer@acme.com", Role = "Viewer", IsActive = true, PasswordHash = "x", CreatedAt = DateTime.UtcNow });
        db.AlertRules.Add(new AlertRule { Id = Guid.NewGuid(), TenantId = viewer.TenantId, SiteId = viewer.SiteId, Name = "R", ThresholdPercent = 10, WindowMinutes = 60, Email = "viewer@acme.com", CooldownMinutes = 60, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        db.SaveChanges();
        var id = db.AlertRules.IgnoreQueryFilters().Single().Id;

        Assert.NotNull(await new AlertRuleService(db, viewer).GetAsync(id, default));
    }

    [Fact]
    public async Task ListAsync_viewer_sees_only_recipient_rules()
    {
        var viewer = new TestCurrentUser { TenantId = _admin.TenantId, SiteId = _admin.SiteId, Role = "Viewer" };
        using var db = InMemoryDb.Create(viewer);
        db.Users.Add(new User { Id = viewer.UserId, TenantId = viewer.TenantId, Email = "viewer@acme.com", Role = "Viewer", IsActive = true, PasswordHash = "x", CreatedAt = DateTime.UtcNow });
        db.AlertRules.AddRange(
            new AlertRule { Id = Guid.NewGuid(), TenantId = viewer.TenantId, SiteId = viewer.SiteId, Name = "Mine", ThresholdPercent = 10, WindowMinutes = 60, Email = "viewer@acme.com", CooldownMinutes = 60, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AlertRule { Id = Guid.NewGuid(), TenantId = viewer.TenantId, SiteId = viewer.SiteId, Name = "NotMine", ThresholdPercent = 10, WindowMinutes = 60, Email = "ops@acme.com", CooldownMinutes = 60, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        db.SaveChanges();

        var list = await new AlertRuleService(db, viewer).ListAsync(default);
        Assert.Single(list);
        Assert.Equal("Mine", list[0].Name);
    }

    [Fact]
    public async Task ListRecipientsAsync_returns_active_site_users()
    {
        using var db = InMemoryDb.Create(_admin);
        var uid = Guid.NewGuid();
        db.Users.Add(new User { Id = uid, TenantId = _admin.TenantId, Email = "u@acme.com", Role = "Viewer", IsActive = true, PasswordHash = "x", CreatedAt = DateTime.UtcNow });
        db.UserSiteAccess.Add(new UserSiteAccess { Id = Guid.NewGuid(), UserId = uid, SiteId = _admin.SiteId, GrantedAt = DateTime.UtcNow });
        db.SaveChanges();

        var recipients = await Sut(db).ListRecipientsAsync(default);
        Assert.Single(recipients);
        Assert.Equal("u@acme.com", recipients[0].Email);
    }

}
