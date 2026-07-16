using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace DocAnalytics.Api.Tests.Security;

public sealed class AuthorizeAuditTests
{
    // Controllers that legitimately allow anonymous access:
    private static readonly HashSet<string> AllowAnonymousControllers = new()
    {
        "AuthController",     // POST /auth/login must be anonymous
        "HealthController",   // GET /health is public (load balancers)
    };

    // Controllers that legitimately use a DIFFERENT policy than DataAccess:
    private static readonly HashSet<string> NotDataAccessControllers = new()
    {
        "ProvisioningController", // DeveloperOnly
        "AdminController",        // ✅ AdminOnly — legitimately not DataAccess
    };

    [Fact]
    public void All_data_controllers_require_DataAccess_policy()
    {
        // ✅ grab the API assembly WITHOUT referencing Program (fixes CS0234)
        var apiAsm = typeof(DocAnalytics.Api.Controllers.HealthController).Assembly;

        var controllers = apiAsm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.Name.EndsWith("Controller", StringComparison.Ordinal));

        foreach (var c in controllers)
        {
            if (AllowAnonymousControllers.Contains(c.Name)) continue;
            if (NotDataAccessControllers.Contains(c.Name)) continue;

            var auth = c.GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            Assert.NotNull(auth); // every data controller MUST be [Authorize]
            Assert.Equal("DataAccess", auth!.Policy);
        }
    }
}
