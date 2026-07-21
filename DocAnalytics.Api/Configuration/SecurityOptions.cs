using System.Diagnostics.CodeAnalysis;
namespace DocAnalytics.Api.Configuration;

/// <summary>Root security configuration bound from the "Security" section (CORS, HSTS, forwarded headers).</summary>
[ExcludeFromCodeCoverage]
public sealed class SecurityOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Security";

    /// <summary>Cross-origin resource sharing settings.</summary>
    public CorsOptions Cors { get; init; } = new();
    /// <summary>HTTP Strict Transport Security settings.</summary>
    public HstsOptions Hsts { get; init; } = new();
    /// <summary>Forwarded-headers settings (for running behind a reverse proxy).</summary>
    public ForwardedHeadersConfig ForwardedHeaders { get; init; } = new();
}

/// <summary>CORS configuration (allowed origins are config-driven).</summary>
public sealed class CorsOptions
{
    /// <summary>The named CORS policy applied by the app.</summary>
    public const string PolicyName = "DocAnalyticsCors";
    /// <summary>Allowed cross-origin origins; empty means no cross-origin access.</summary>
    public string[] AllowedOrigins { get; init; } = [];
}

/// <summary>HSTS configuration.</summary>
public sealed class HstsOptions
{
    /// <summary>Whether HSTS is enabled.</summary>
    public bool Enabled { get; init; }
    /// <summary>Max-age in days.</summary>
    public int MaxAgeDays { get; init; } = 365;
    /// <summary>Whether to include subdomains.</summary>
    public bool IncludeSubDomains { get; init; } = true;
    /// <summary>Whether to set the preload directive.</summary>
    public bool Preload { get; init; }
}

/// <summary>Forwarded-headers configuration for correct scheme/IP detection behind a proxy.</summary>
public sealed class ForwardedHeadersConfig
{
    /// <summary>Whether forwarded-headers processing is enabled.</summary>
    public bool Enabled { get; init; } = true;
}
