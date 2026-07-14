using Microsoft.AspNetCore.HttpOverrides;

namespace DocAnalytics.Api.Configuration;

public static class SecurityFoundationExtensions
{
    /// <summary>
    /// Round 0 foundation: binds SecurityOptions, registers the CORS policy,
    /// HSTS knobs, and forwarded-headers config. The pipeline wiring
    /// (UseCors / UseHsts / UseForwardedHeaders) is done in R1 transport hardening.
    /// </summary>
    public static IServiceCollection AddSecurityFoundation(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SecurityOptions>(config.GetSection(SecurityOptions.SectionName));
        var opts = config.GetSection(SecurityOptions.SectionName).Get<SecurityOptions>()
                   ?? new SecurityOptions();

        // ---- CORS (origins are config-driven, never hardcoded) ----
        services.AddCors(o => o.AddPolicy(CorsOptions.PolicyName, policy =>
        {
            if (opts.Cors.AllowedOrigins.Length == 0) return;   // no origins => no cross-origin allowed
            policy.WithOrigins(opts.Cors.AllowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();   // SignalR + the R5 httpOnly-cookie option need this
        }));

        // ---- HSTS knobs (UseHsts() itself is wired in R1) ----
        if (opts.Hsts.Enabled)
        {
            services.AddHsts(h =>
            {
                h.MaxAge = TimeSpan.FromDays(opts.Hsts.MaxAgeDays);
                h.IncludeSubDomains = opts.Hsts.IncludeSubDomains;
                h.Preload = opts.Hsts.Preload;
            });
        }

        // ---- Forwarded headers (so Kestrel behind nginx sees the real scheme) ----
        if (opts.ForwardedHeaders.Enabled)
        {
            services.Configure<ForwardedHeadersOptions>(f =>
            {
                f.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                                   | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
                // nginx is a single trusted hop inside the compose network.
                f.KnownIPNetworks.Clear();   // was KnownNetworks (deprecated in .NET 10)
                f.KnownProxies.Clear();
            });
        }

        return services;
    }
}
