using DocAnalytics.Domain.Common;

namespace DocAnalytics.Domain.Entities;

/// <summary>
/// A fired alert, persisted so it can surface as an in-app notification on login.
/// Tenant/site-scoped via <see cref="ITenantScoped"/> (background inserts set the scope explicitly).
/// </summary>
// A fired alert, persisted so it can surface as an in-app notification on login.
// ITenantScoped → global query filter auto-applies WHERE tenant_id = X AND site_id = Y
// on every READ. (Inserts from the background evaluator set these explicitly.)
public class AlertNotification : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }

    /// <summary>The alert rule that fired.</summary>
    public Guid AlertRuleId { get; set; }               // which rule fired
    /// <summary>Denormalized rule name (avoids a JOIN on display).</summary>
    public string RuleName { get; set; } = null!;       // denormalized → no JOIN on display
    /// <summary>Human-readable summary message.</summary>
    public string Message { get; set; } = null!;        // human-readable summary
    /// <summary>Severity: info | warning | critical.</summary>
    public string Severity { get; set; } = "warning";   // info | warning | critical

    /// <summary>The observed failure percentage that tripped the rule.</summary>
    public double ObservedPercent { get; set; }         // failure % that tripped it
    /// <summary>The rule's threshold percentage at fire time.</summary>
    public double ThresholdPercent { get; set; }        // the rule's threshold at fire time

    /// <summary>Whether the notification has been read.</summary>
    public bool IsRead { get; set; }
    /// <summary>When the alert fired (UTC).</summary>
    public DateTime FiredAt { get; set; }
    /// <summary>When the notification was read (UTC), if read.</summary>
    public DateTime? ReadAt { get; set; }
}
