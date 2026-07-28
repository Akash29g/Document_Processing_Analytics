using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Service.Auth;

/// <summary>Issues, rotates, and revokes opaque refresh tokens (only their hashes are persisted).</summary>
public interface IRefreshTokenService
{
    /// <summary>Mints a new opaque refresh token for the user and persists its hash.
    /// Returns the RAW token (shown once) + its expiry.</summary>
    Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(
        Guid userId, string? ip, string? userAgent = null, CancellationToken ct = default);

    /// <summary>Validates a presented raw token. On success ROTATES it (revokes old, issues new)
    /// and returns the owning user + the new raw token. Returns null if invalid/expired/revoked.
    /// Reuse of an already-revoked token revokes the whole chain for that user.</summary>
    Task<(User User, string RawToken, DateTime ExpiresAt)?> ValidateAndRotateAsync(
        string rawToken, string? ip, string? userAgent = null, CancellationToken ct = default);

    /// <summary>Revokes a single token (logout).</summary>
    Task RevokeAsync(string rawToken, CancellationToken ct = default);

    /// <summary>Revokes every active token for a user (e.g. password change / force logout).</summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Lists this user's active (non-revoked, non-expired) sessions, newest-active first.</summary>
    Task<IReadOnlyList<SessionDto>> ListActiveSessionsAsync(Guid userId, string? currentRawToken, CancellationToken ct = default);

    /// <summary>Revokes ONE session. Scoped by userId — a user can never revoke someone else's session.</summary>
    Task<bool> RevokeSessionAsync(Guid userId, Guid tokenId, CancellationToken ct = default);

    /// <summary>Revokes every session EXCEPT the current one ("log out everywhere else"). Returns the count revoked.</summary>
    Task<int> RevokeAllOtherSessionsAsync(Guid userId, string currentRawToken, CancellationToken ct = default);
}
