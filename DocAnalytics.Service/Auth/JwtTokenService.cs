using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocAnalytics.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DocAnalytics.Service.Auth;

/// <summary>Default <see cref="IJwtTokenService"/> implementation: builds HMAC-SHA256 signed JWTs from configuration.</summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    /// <inheritdoc />
    public string CreateToken(User user)
    {
        // Secret comes from user-secrets. Must be >= 32 chars or startup throws IDX10720.
        var keyString = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 15;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // The claims = the info "printed on the wristband"
        var claims = new List<Claim>
        {
            new("userId",   user.Id.ToString()),
            new("tenantId", user.TenantId?.ToString() ?? string.Empty),
            new("role",     user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],     // 👈 "DocAnalytics"
            audience: _config["Jwt:Audience"],   // 👈 "DocAnalyticsClient"
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
