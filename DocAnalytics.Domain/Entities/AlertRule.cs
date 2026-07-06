using DocAnalytics.Domain.Common;   // ITenantScoped

namespace DocAnalytics.Domain.Entities;

// S-4 — email alert when a site's failure rate exceeds a threshold
public class AlertRule : ITenantScoped
{
    public Guid Id { get; set; }

    // satisfies ITenantScoped → global tenant/site query filter auto-applies
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }

    public string Name { get; set; } = null!;        // "High failure alert"
    public double ThresholdPercent { get; set; }      // fire if failure rate > this (e.g. 10)
    public int WindowMinutes { get; set; } = 60;      // look-back window
    public string Email { get; set; } = null!;        // recipient(s), comma-separated
    public bool IsEnabled { get; set; } = true;

    public int CooldownMinutes { get; set; } = 60;    // min gap between emails
    public DateTime? LastTriggeredAt { get; set; }    // UTC, null until first fire

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
