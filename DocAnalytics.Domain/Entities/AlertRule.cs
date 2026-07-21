using DocAnalytics.Domain.Common;   // ITenantScoped

namespace DocAnalytics.Domain.Entities;

/// <summary>A per-site rule (S-4) that emails an alert when the failure rate exceeds a threshold; tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
// S-4 — email alert when a site's failure rate exceeds a threshold
public class AlertRule : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }

    /// <summary>Display name (e.g. "High failure alert").</summary>
    public string Name { get; set; } = null!;        // "High failure alert"
    /// <summary>Fire when the failure rate exceeds this percentage (e.g. 10).</summary>
    public double ThresholdPercent { get; set; }      // fire if failure rate > this (e.g. 10)
    /// <summary>Look-back window in minutes.</summary>
    public int WindowMinutes { get; set; } = 60;      // look-back window
    /// <summary>Recipient email(s), comma-separated.</summary>
    public string Email { get; set; } = null!;        // recipient(s), comma-separated
    /// <summary>Whether the rule is enabled.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Minimum gap between emails, in minutes.</summary>
    public int CooldownMinutes { get; set; } = 60;    // min gap between emails
    /// <summary>When the rule last fired (UTC); null until first fire.</summary>
    public DateTime? LastTriggeredAt { get; set; }    // UTC, null until first fire

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Last update timestamp (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}
