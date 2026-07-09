using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DocAnalytics.Service.Tests.Support;

// Strict-mock AppDbContext: no real DB, no provider, no tenant filter.
// Each test seeds only the DbSets its service touches.
public static class MockDb
{
    public static Mock<AppDbContext> Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().Options;
        return new Mock<AppDbContext>(options, Mock.Of<ICurrentUser>());
    }
}
