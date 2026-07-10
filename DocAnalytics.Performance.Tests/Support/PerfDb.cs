using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Performance.Tests.Support;

public static class PerfDb
{
    public static AppDbContext Create(ICurrentUser user, string name) =>
        new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(name)
                .Options,
            user);
}
