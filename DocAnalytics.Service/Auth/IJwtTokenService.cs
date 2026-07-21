using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Service.Auth;

/// <summary>Issues signed JWT access tokens for authenticated users.</summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed JWT carrying the user's id, tenant, and role claims.</summary>
    /// <param name="user">The user to mint a token for.</param>
    /// <returns>The encoded JWT string.</returns>
    string CreateToken(User user);
}
