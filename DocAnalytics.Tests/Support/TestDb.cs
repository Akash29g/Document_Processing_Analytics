using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Tests.Support;

public static class TestDb
{
    // A fresh, isolated in-memory database per call (unique name = no cross-test bleed).
    public static AppDbContext Create(ICurrentUser user) =>
        new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options,
            user);
}
