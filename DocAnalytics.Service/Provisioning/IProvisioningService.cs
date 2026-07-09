namespace DocAnalytics.Service.Provisioning;

public interface IProvisioningService
{
    Task<List<TenantSummaryDto>> GetTenantsAsync(CancellationToken ct);
    Task<TenantSummaryDto?> CreateTenantAsync(CreateTenantRequest req, CancellationToken ct);   // null = domain taken

    Task<List<ProvisionedUserDto>> GetUsersAsync(Guid tenantId, CancellationToken ct);
    Task<ProvisionedUserDto?> CreateAdminAsync(Guid tenantId, CreateAdminRequest req, Guid createdBy, CancellationToken ct);
    Task<bool> RemoveAdminAsync(Guid tenantId, Guid userId, CancellationToken ct);
    Task<bool> RemoveUserAsync(Guid tenantId, Guid userId, CancellationToken ct);

    Task<List<ProvisionedSiteDto>> GetSitesAsync(Guid tenantId, CancellationToken ct);
    Task<ProvisionedSiteDto?> CreateSiteAsync(Guid tenantId, CreateSiteRequest req, CancellationToken ct);
    Task<bool> RemoveSiteAsync(Guid tenantId, Guid siteId, CancellationToken ct);
}
