namespace DocAnalytics.Service.Auth;

public interface ILoginLockoutService
{
    Task<(bool Locked, int RetryAfterSeconds)> IsLockedAsync(string email, CancellationToken ct = default);
    Task RegisterFailureAsync(string email, string? ip, CancellationToken ct = default);
    Task ResetAsync(string email, CancellationToken ct = default);
}
