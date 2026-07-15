using System.Diagnostics.CodeAnalysis;
namespace DocAnalytics.Api.Configuration;


[ExcludeFromCodeCoverage]

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public CorsOptions Cors { get; init; } = new();
    public HstsOptions Hsts { get; init; } = new();
    public ForwardedHeadersConfig ForwardedHeaders { get; init; } = new();
}

public sealed class CorsOptions
{
    public const string PolicyName = "DocAnalyticsCors";
    public string[] AllowedOrigins { get; init; } = [];
}

public sealed class HstsOptions
{
    public bool Enabled { get; init; }
    public int MaxAgeDays { get; init; } = 365;
    public bool IncludeSubDomains { get; init; } = true;
    public bool Preload { get; init; }
}

public sealed class ForwardedHeadersConfig
{
    public bool Enabled { get; init; } = true;
}
