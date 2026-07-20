namespace DocAnalytics.Api.Configuration;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public bool Enabled { get; set; } = true;
    public RateLimitPolicySettings Login { get; set; } = new() { PermitLimit = 5, WindowSeconds = 60 };
    public RateLimitPolicySettings Reads { get; set; } = new() { PermitLimit = 100, WindowSeconds = 60 };
    public RateLimitPolicySettings Export { get; set; } = new() { PermitLimit = 3, WindowSeconds = 60 };
}

public sealed class RateLimitPolicySettings
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
    public int QueueLimit { get; set; } = 0;   // 0 = reject immediately, no queueing
}
