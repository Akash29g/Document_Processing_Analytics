using System.Net;
using System.Text;
using DocAnalytics.Api.BackgroundServices;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocAnalytics.Api.Tests.RateLimiting;

// Uses HealthController only to locate the API assembly — no need to expose Program.
public sealed class RateLimitFactory : WebApplicationFactory<HealthController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");   // not Development → skips Swagger + DbSeeder

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // pass Program.cs guards
                ["ConnectionStrings:Default"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["Jwt:Key"] = "test-signing-key-that-is-at-least-32-chars-long!!",
                ["Jwt:Issuer"] = "DocAnalytics",
                ["Jwt:Audience"] = "DocAnalyticsClient",
                // deterministic tiny login limit (this is why B5 config-driven matters)
                ["RateLimiting:Login:PermitLimit"] = "5",
                ["RateLimiting:Login:WindowSeconds"] = "60",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // swap Npgsql AppDbContext → in-memory (fast, no real DB)
            var dbOpts = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbOpts is not null) services.Remove(dbOpts);
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("ratelimit-tests"));

            // drop background workers so they don't hit the DB during the test
            foreach (var d in services.Where(d =>
                         d.ImplementationType == typeof(AlertEvaluationBackgroundService) ||
                         d.ImplementationType == typeof(ExtractionWorker)).ToList())
                services.Remove(d);
        });
    }
}

public sealed class RateLimitTests : IClassFixture<RateLimitFactory>
{
    private readonly RateLimitFactory _factory;
    public RateLimitTests(RateLimitFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_returns_429_after_limit_exceeded()
    {
        var client = _factory.CreateClient();

        HttpResponseMessage? last = null;
        for (var i = 0; i < 7; i++)   // PermitLimit = 5 → 6th & 7th rejected by the limiter
        {
            last = await client.PostAsync("/api/v1/auth/login",
                new StringContent("""{ "email": "no@one.com", "password": "wrong" }""",
                    Encoding.UTF8, "application/json"));
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, last!.StatusCode);
        Assert.True(last.Headers.Contains("Retry-After"));
    }
}
