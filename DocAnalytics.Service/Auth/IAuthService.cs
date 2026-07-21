namespace DocAnalytics.Service.Auth;

/// <summary>Authentication and account operations: login, profile, authorized sites, and password change (FR-5).</summary>
public interface IAuthService
{
    /// <summary>Verifies credentials and returns the login payload (user, token data, sites).</summary>
    /// <param name="req">Login request with email and password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The login response, or <c>null</c> if the credentials are invalid.</returns>
    Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct);

    /// <summary>Returns the current user's profile and authorized sites for session rehydration.</summary>
    /// <param name="userId">The current user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The profile, or <c>null</c> if the user no longer exists.</returns>
    Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct);

    /// <summary>Returns the sites a user is authorized to access (FR-5.1).</summary>
    /// <param name="userId">The user id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user's authorized sites.</returns>
    Task<IReadOnlyList<SiteDto>> GetSitesAsync(Guid userId, CancellationToken ct);

    /// <summary>Changes a user's password after verifying the current one.</summary>
    /// <param name="userId">The user id.</param>
    /// <param name="req">Current and new password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if changed; <c>false</c> if the current password is incorrect.</returns>
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct);

}
