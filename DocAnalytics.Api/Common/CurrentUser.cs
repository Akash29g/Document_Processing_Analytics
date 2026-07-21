using DocAnalytics.Domain.Common;

namespace DocAnalytics.Api.Common;

/// <summary>Request-scoped <see cref="ICurrentUser"/> implementation, populated from JWT claims by the tenant/site middleware.</summary>
public class CurrentUser : ICurrentUser
{
    /// <inheritdoc />
    public Guid UserId { get; private set; }
    /// <inheritdoc />
    public Guid TenantId { get; private set; }
    /// <inheritdoc />
    public Guid SiteId { get; private set; }
    /// <inheritdoc />
    public string Role { get; private set; } = string.Empty;
    /// <inheritdoc />
    public bool IsAuthenticated { get; private set; }

    /// <summary>Populates the identity/tenancy context for the current request and marks it authenticated.</summary>
    /// <param name="userId">The authenticated user's id.</param>
    /// <param name="tenantId">The active tenant id.</param>
    /// <param name="siteId">The active site id.</param>
    /// <param name="role">The user's role.</param>
    public void Set(Guid userId, Guid tenantId, Guid siteId, string role)
    {
        UserId = userId; TenantId = tenantId; SiteId = siteId; Role = role; IsAuthenticated = true;
    }
}
