using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocAnalytics.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DocAnalytics.Api.Auth;

public class JwtTokenService
{
    private readonly JwtSettings _s;
    public JwtTokenService(IOptions<JwtSettings> s) => _s = s.Value;

    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_s.Key));
        var token = new JwtSecurityToken(_s.Issuer, _s.Audience, claims,
            expires: DateTime.UtcNow.AddMinutes(_s.ExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
