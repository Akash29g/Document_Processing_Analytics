namespace DocAnalytics.Service.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct);
    Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<SiteDto>> GetSitesAsync(Guid userId, CancellationToken ct);
}
