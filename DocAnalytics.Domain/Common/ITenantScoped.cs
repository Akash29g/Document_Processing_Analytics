namespace DocAnalytics.Domain.Common;

// Marker for tables carrying BOTH tenant_id + site_id -> auto global filter
public interface ITenantScoped
{
    Guid TenantId { get; }
    Guid SiteId { get; }
}
