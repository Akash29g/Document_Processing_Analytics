namespace DocAnalytics.Service.Provisioning;

/// <summary>Platform-level provisioning of tenants, their admins/users, and sites (Developer role).</summary>
public interface IProvisioningService
{
    /// <summary>Lists all tenants with summary counts.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>All tenants.</returns>
    Task<List<TenantSummaryDto>> GetTenantsAsync(CancellationToken ct);

    /// <summary>Creates a new tenant.</summary>
    /// <param name="req">Tenant name and org domain.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created tenant, or <c>null</c> if the org domain is already taken.</returns>
    Task<TenantSummaryDto?> CreateTenantAsync(CreateTenantRequest req, CancellationToken ct);   // null = domain taken

    /// <summary>Lists the users belonging to a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's users.</returns>
    Task<List<ProvisionedUserDto>> GetUsersAsync(Guid tenantId, CancellationToken ct);

    /// <summary>Creates an admin user in a tenant and emails generated credentials.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="req">First/last name for the admin.</param>
    /// <param name="createdBy">The id of the Developer performing the action.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created admin, or <c>null</c> if the tenant is not found or inactive.</returns>
    Task<ProvisionedUserDto?> CreateAdminAsync(Guid tenantId, CreateAdminRequest req, Guid createdBy, CancellationToken ct);

    /// <summary>Removes (deactivates) an admin from a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="userId">The admin user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if removed; <c>false</c> if not found in the tenant.</returns>
    Task<bool> RemoveAdminAsync(Guid tenantId, Guid userId, CancellationToken ct);

    /// <summary>Removes (deactivates) a user from a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if removed; <c>false</c> if not found in the tenant.</returns>
    Task<bool> RemoveUserAsync(Guid tenantId, Guid userId, CancellationToken ct);

    /// <summary>Lists the sites belonging to a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's sites.</returns>
    Task<List<ProvisionedSiteDto>> GetSitesAsync(Guid tenantId, CancellationToken ct);

    /// <summary>Creates a new site in a tenant.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="req">Site name and location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created site, or <c>null</c> if the tenant is not found or inactive.</returns>
    Task<ProvisionedSiteDto?> CreateSiteAsync(Guid tenantId, CreateSiteRequest req, CancellationToken ct);

    /// <summary>Removes (deactivates) a site from a tenant and revokes access to it.</summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="siteId">The site id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if removed; <c>false</c> if not found in the tenant.</returns>
    Task<bool> RemoveSiteAsync(Guid tenantId, Guid siteId, CancellationToken ct);
}
