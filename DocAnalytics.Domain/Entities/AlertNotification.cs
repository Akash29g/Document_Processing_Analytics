using DocAnalytics.Domain.Common;

namespace DocAnalytics.Domain.Entities;

// A fired alert, persisted so it can surface as an in-app notification on login.
// ITenantScoped → global query filter auto-applies WHERE tenant_id = X AND site_id = Y
// on every READ. (Inserts from the background evaluator set these explicitly.)
public class AlertNotification : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }

    public Guid AlertRuleId { get; set; }               // which rule fired
    public string RuleName { get; set; } = null!;       // denormalized → no JOIN on display
    public string Message { get; set; } = null!;        // human-readable summary
    public string Severity { get; set; } = "warning";   // info | warning | critical

    public double ObservedPercent { get; set; }         // failure % that tripped it
    public double ThresholdPercent { get; set; }        // the rule's threshold at fire time

    public bool IsRead { get; set; }
    public DateTime FiredAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
