using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Tests.Auth;

public class LoginLockoutServiceTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"lockout-{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options, new StubCurrentUser());
    }

    // LoginAttempt is NOT tenant-scoped, so the tenant values here are irrelevant.
    private sealed class StubCurrentUser : ICurrentUser
    {
        public Guid UserId => Guid.Empty;
        public Guid TenantId => Guid.Empty;
        public Guid SiteId => Guid.Empty;
        public string Role => "";
        public bool IsAuthenticated => false;
    }

    [Fact]
    public async Task Not_locked_before_any_failures()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        var (locked, _) = await svc.IsLockedAsync("a@org.com");
        Assert.False(locked);
    }

    [Fact]
    public async Task Stays_unlocked_at_four_failures()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        for (var i = 0; i < 4; i++) await svc.RegisterFailureAsync("a@org.com", "1.2.3.4");
        var (locked, _) = await svc.IsLockedAsync("a@org.com");
        Assert.False(locked);
    }

    [Fact]
    public async Task Locks_after_five_failures_with_sane_retry_after()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        for (var i = 0; i < 5; i++) await svc.RegisterFailureAsync("a@org.com", "1.2.3.4");
        var (locked, retryAfter) = await svc.IsLockedAsync("a@org.com");
        Assert.True(locked);
        Assert.InRange(retryAfter, 1, 15 * 60);   // within the 15-minute lock
    }

    [Fact]
    public async Task Reset_clears_the_counter_and_row()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        for (var i = 0; i < 5; i++) await svc.RegisterFailureAsync("a@org.com", "1.2.3.4");
        await svc.ResetAsync("a@org.com");
        var (locked, _) = await svc.IsLockedAsync("a@org.com");
        Assert.False(locked);
        Assert.False(await db.LoginAttempts.AnyAsync());
    }

    [Fact]
    public async Task Email_is_case_and_whitespace_insensitive()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        for (var i = 0; i < 5; i++) await svc.RegisterFailureAsync("  A@ORG.com ", "1.2.3.4");
        var (locked, _) = await svc.IsLockedAsync("a@org.com");   // normalized match
        Assert.True(locked);
    }

    [Fact]
    public async Task Counter_rearms_after_window_expires()
    {
        using var db = NewDb();
        var svc = new LoginLockoutService(db);
        await svc.RegisterFailureAsync("a@org.com", null);
        await svc.RegisterFailureAsync("a@org.com", null);   // count = 2

        // Simulate the 15-min window having elapsed.
        var row = await db.LoginAttempts.FirstAsync();
        row.FirstFailedAt = DateTime.UtcNow.AddMinutes(-20);
        await db.SaveChangesAsync();

        await svc.RegisterFailureAsync("a@org.com", null);   // window expired → reset to 0, then +1
        row = await db.LoginAttempts.FirstAsync();
        Assert.Equal(1, row.FailedCount);
    }
}
