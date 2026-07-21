namespace DocAnalytics.Domain.Common;

/// <summary>
/// Ambient accessor for the authenticated request's identity and tenancy context.
/// Populated per-request (from the JWT / tenant-site middleware) and consumed by services
/// to stamp tenant/site on inserts and drive authorization decisions.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The authenticated user's id.</summary>
    Guid UserId { get; }
    /// <summary>The active tenant id for this request.</summary>
    Guid TenantId { get; }
    /// <summary>The active site id for this request.</summary>
    Guid SiteId { get; }
    /// <summary>The user's role: Developer | Admin | Viewer.</summary>
    string Role { get; }
    /// <summary>Whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}
