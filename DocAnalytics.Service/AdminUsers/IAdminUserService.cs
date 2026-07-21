namespace DocAnalytics.Service.AdminUsers;

/// <summary>Tenant-admin operations for managing Viewer users and sites within the caller's own tenant.</summary>
public interface IAdminUserService
{
    /// <summary>Lists the Viewer users in the caller's tenant.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's users.</returns>
    Task<List<AdminUserDto>> GetUsersAsync(CancellationToken ct);

    /// <summary>Creates a Viewer user and emails generated credentials.</summary>
    /// <param name="req">First/last name and the site ids to grant.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user, or <c>null</c> if any site id is invalid for the tenant.</returns>
    Task<AdminCreatedUserDto?> CreateUserAsync(AdminCreateUserRequest req, CancellationToken ct);  // null = bad site ids

    /// <summary>Replaces the set of sites a user may access.</summary>
    /// <param name="userId">The target user id.</param>
    /// <param name="req">The new set of site ids.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if updated; <c>false</c> if the user isn't in the tenant or site ids are invalid.</returns>
    Task<bool> UpdateUserSitesAsync(Guid userId, UpdateUserSitesRequest req, CancellationToken ct);

    /// <summary>Deactivates (soft-deletes) a Viewer user in the caller's tenant.</summary>
    /// <param name="userId">The target user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if deactivated; <c>false</c> if not found in the tenant.</returns>
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct);

    /// <summary>Lists the sites in the caller's tenant.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tenant's sites.</returns>
    Task<List<AdminSiteDto>> GetSitesAsync(CancellationToken ct);

    /// <summary>Creates a new site in the caller's tenant and grants the creator access.</summary>
    /// <param name="req">Site name and location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created site.</returns>
    Task<AdminSiteDto> CreateSiteAsync(AdminCreateSiteRequest req, CancellationToken ct);
}
