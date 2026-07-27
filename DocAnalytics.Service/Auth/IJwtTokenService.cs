using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Service.Auth;

/// <summary>Issues signed JWT access tokens for authenticated users.</summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed JWT carrying the user's id, tenant, and role claims.</summary>
    /// <param name="user">The user to mint a token for.</param>
    /// <returns>The encoded JWT string.</returns>
    string CreateToken(User user);

    /// <summary>Creates a short-lived (5 min), purpose-scoped token identifying a user mid-login,
    /// used only to complete a 2FA challenge. Carries no role/tenant claims.</summary>
    string CreateTwoFactorChallengeToken(Guid userId);

    /// <summary>Validates a 2FA challenge token. Returns the embedded user id, or null if invalid/expired/wrong purpose.</summary>
    Guid? ValidateTwoFactorChallengeToken(string token);
}
