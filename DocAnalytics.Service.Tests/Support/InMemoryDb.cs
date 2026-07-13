using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Tests.Support;

// Real AppDbContext on an isolated in-memory store — for services that WRITE
// (Add/Remove/SaveChanges), which the query-only MockDb can't exercise.
public sealed class TestCurrentUser : ICurrentUser
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; } = Guid.NewGuid();
    public Guid SiteId { get; init; } = Guid.NewGuid();
    public string Role { get; init; } = "Admin";
    public bool IsAuthenticated { get; init; } = true;
}

public static class InMemoryDb
{
    public static AppDbContext Create(ICurrentUser user) =>
        new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())   // unique = no cross-test bleed
                .Options,
            user);
}
