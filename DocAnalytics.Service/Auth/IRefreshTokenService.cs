using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Service.Auth;

public interface IRefreshTokenService
{
    /// <summary>Mints a new opaque refresh token for the user and persists its hash.
    /// Returns the RAW token (shown once) + its expiry.</summary>
    Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(Guid userId, string? ip, CancellationToken ct = default);

    /// <summary>Validates a presented raw token. On success ROTATES it (revokes old, issues new)
    /// and returns the owning user + the new raw token. Returns null if invalid/expired/revoked.
    /// Reuse of an already-revoked token revokes the whole chain for that user.</summary>
    Task<(User User, string RawToken, DateTime ExpiresAt)?> ValidateAndRotateAsync(
        string rawToken, string? ip, CancellationToken ct = default);

    /// <summary>Revokes a single token (logout).</summary>
    Task RevokeAsync(string rawToken, CancellationToken ct = default);

    /// <summary>Revokes every active token for a user (e.g. password change / force logout).</summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
}
