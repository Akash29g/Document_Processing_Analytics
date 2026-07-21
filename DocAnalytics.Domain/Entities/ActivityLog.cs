// Entities/ActivityLog.cs
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;

/// <summary>An audit-trail entry recording a state change or event; tenant/site-scoped via <see cref="ITenantScoped"/>.</summary>
public class ActivityLog : ITenantScoped
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }
    /// <summary>Owning tenant; enforced by the global query filter.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Owning site; enforced by the global query filter.</summary>
    public Guid SiteId { get; set; }
    /// <summary>The event type (e.g. FILE_STATE_CHANGED, BATCH_SUBMITTED).</summary>
    public string EventType { get; set; } = null!;
    /// <summary>The affected entity type (e.g. File, Batch).</summary>
    public string EntityType { get; set; } = null!;
    /// <summary>The affected entity's id.</summary>
    public Guid EntityId { get; set; }
    /// <summary>The affected entity's display name, if available.</summary>
    public string? EntityName { get; set; }
    /// <summary>Prior state, if applicable.</summary>
    public string? OldState { get; set; }
    /// <summary>New state, if applicable.</summary>
    public string? NewState { get; set; }
    /// <summary>Who or what triggered the event (e.g. user, simulator).</summary>
    public string TriggeredBy { get; set; } = null!;
    /// <summary>When the event occurred (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}
