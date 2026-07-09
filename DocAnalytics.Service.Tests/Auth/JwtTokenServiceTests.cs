using System.IdentityModel.Tokens.Jwt;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class JwtTokenServiceTests
{
    private static IConfiguration Config(string? key = "super-secret-signing-key-at-least-32-chars!!")
    {
        var cfg = new Mock<IConfiguration>();
        cfg.Setup(c => c["Jwt:Key"]).Returns(key!);
        cfg.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("120");
        cfg.Setup(c => c["Jwt:Issuer"]).Returns("DocAnalytics");
        cfg.Setup(c => c["Jwt:Audience"]).Returns("DocAnalyticsClient");
        return cfg.Object;
    }

    private static User SampleUser()
        => new() { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), Email = "a@org.com", Role = "Admin" };

    [Fact]
    public void CreateToken_returns_parseable_jwt_with_expected_claims()
    {
        var user = SampleUser();
        var token = new JwtTokenService(Config()).CreateToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("DocAnalytics", jwt.Issuer);
        Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == "userId").Value);
        Assert.Equal(user.TenantId.ToString(), jwt.Claims.First(c => c.Type == "tenantId").Value);
        Assert.Equal("Admin", jwt.Claims.First(c => c.Type == "role").Value);
    }

    [Fact]
    public void CreateToken_sets_configured_audience()
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(new JwtTokenService(Config()).CreateToken(SampleUser()));
        Assert.Contains("DocAnalyticsClient", jwt.Audiences);
    }

    [Fact]
    public void CreateToken_throws_when_key_missing()
    {
        var cfg = new Mock<IConfiguration>();
        cfg.Setup(c => c["Jwt:Key"]).Returns((string?)null);
        Assert.Throws<InvalidOperationException>(() => new JwtTokenService(cfg.Object).CreateToken(SampleUser()));
    }
}
