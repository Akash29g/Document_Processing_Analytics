namespace DocAnalytics.Api.Configuration;

/// <summary>Rate-limiting configuration bound from the "RateLimiting" section (Round 5).</summary>
public sealed class RateLimitOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "RateLimiting";

    /// <summary>Master on/off switch for rate limiting.</summary>
    public bool Enabled { get; set; } = true;
    /// <summary>Limits for the login policy (per client IP).</summary>
    public RateLimitPolicySettings Login { get; set; } = new() { PermitLimit = 5, WindowSeconds = 60 };
    /// <summary>Limits for read endpoints (per authenticated user).</summary>
    public RateLimitPolicySettings Reads { get; set; } = new() { PermitLimit = 100, WindowSeconds = 60 };
    /// <summary>Limits for export endpoints (tight, per user).</summary>
    public RateLimitPolicySettings Export { get; set; } = new() { PermitLimit = 3, WindowSeconds = 60 };
}

/// <summary>Fixed-window limit settings for a single rate-limit policy.</summary>
public sealed class RateLimitPolicySettings
{
    /// <summary>Maximum requests permitted per window.</summary>
    public int PermitLimit { get; set; }
    /// <summary>Window length in seconds.</summary>
    public int WindowSeconds { get; set; }
    /// <summary>Queue length for waiting requests; 0 rejects immediately (no queueing).</summary>
    public int QueueLimit { get; set; } = 0;   // 0 = reject immediately, no queueing
}
