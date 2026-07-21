namespace DocAnalytics.Service.Auth;

/// <summary>Account-level brute-force protection: tracks failed logins and locks an account temporarily.</summary>
public interface ILoginLockoutService
{
    /// <summary>Checks whether an account is currently locked out.</summary>
    /// <param name="email">The account email (matched case-insensitively).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple: whether the account is locked, and seconds until it unlocks.</returns>
    Task<(bool Locked, int RetryAfterSeconds)> IsLockedAsync(string email, CancellationToken ct = default);

    /// <summary>Records a failed login attempt, locking the account once the failure threshold is reached.</summary>
    /// <param name="email">The account email.</param>
    /// <param name="ip">The client IP, if known.</param>
    /// <param name="ct">Cancellation token.</param>
    Task RegisterFailureAsync(string email, string? ip, CancellationToken ct = default);

    /// <summary>Clears the failure counter/lock for an account after a successful login.</summary>
    /// <param name="email">The account email.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ResetAsync(string email, CancellationToken ct = default);
}
