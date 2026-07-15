// Entities/ActivityLog.cs
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class ActivityLog : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string EventType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? OldState { get; set; }
    public string? NewState { get; set; }
    public string TriggeredBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
