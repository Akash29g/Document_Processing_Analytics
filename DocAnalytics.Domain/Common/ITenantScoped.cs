namespace DocAnalytics.Domain.Common;

/// <summary>
/// Marker for entities carrying BOTH <c>tenant_id</c> and <c>site_id</c>. Implementing this
/// opts an entity into the EF Core global query filter, which automatically appends
/// <c>WHERE tenant_id = X AND site_id = Y</c> to every read for the current user.
/// </summary>
// Marker for tables carrying BOTH tenant_id + site_id -> auto global filter
public interface ITenantScoped
{
    /// <summary>The owning tenant id.</summary>
    Guid TenantId { get; }
    /// <summary>The owning site id.</summary>
    Guid SiteId { get; }
}
