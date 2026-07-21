namespace DocAnalytics.Api.Auth;

/// <summary>Strongly-typed JWT configuration bound from the "Jwt" settings section.</summary>
public class JwtSettings
{
    /// <summary>The token issuer (iss).</summary>
    public string Issuer { get; set; } = null!;
    /// <summary>The intended token audience (aud).</summary>
    public string Audience { get; set; } = null!;
    /// <summary>The symmetric signing key.</summary>
    public string Key { get; set; } = null!;
    /// <summary>Access-token lifetime in minutes.</summary>
    public int ExpiryMinutes { get; set; } = 120;
}
