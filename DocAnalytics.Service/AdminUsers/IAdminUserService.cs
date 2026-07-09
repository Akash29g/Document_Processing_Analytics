namespace DocAnalytics.Service.AdminUsers;

public interface IAdminUserService
{
    Task<List<AdminUserDto>> GetUsersAsync(CancellationToken ct);
    Task<AdminCreatedUserDto?> CreateUserAsync(AdminCreateUserRequest req, CancellationToken ct);  // null = bad site ids
    Task<bool> UpdateUserSitesAsync(Guid userId, UpdateUserSitesRequest req, CancellationToken ct);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct);

    Task<List<AdminSiteDto>> GetSitesAsync(CancellationToken ct);
    Task<AdminSiteDto> CreateSiteAsync(AdminCreateSiteRequest req, CancellationToken ct);
}
