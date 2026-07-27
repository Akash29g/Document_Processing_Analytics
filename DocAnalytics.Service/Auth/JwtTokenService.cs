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

    /// <inheritdoc />
    public string CreateTwoFactorChallengeToken(Guid userId)
    {
        var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Deliberately NO role/tenantId claims — this token proves "who", not "what they can do".
        var claims = new List<Claim>
        {
            new("userId", userId.ToString()),
            new("purpose", "2fa_challenge"),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public Guid? ValidateTwoFactorChallengeToken(string token)
    {
        var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString)),
            }, out _);

            if (principal.FindFirst("purpose")?.Value != "2fa_challenge") return null;
            var idStr = principal.FindFirst("userId")?.Value;
            return Guid.TryParse(idStr, out var id) ? id : null;
        }
        catch
        {
            return null; // expired/tampered/wrong purpose — reject silently
        }
    }
}
