namespace DocAnalytics.Service.Auth;

/// <summary>Forgot/Reset password flow: issue a one-time reset link and consume it to set a new password.</summary>
public interface IPasswordResetService
{
    /// <summary>Always completes silently (never reveals whether the email exists). Emails a reset link if the account is valid.</summary>
    Task RequestResetAsync(ForgotPasswordRequest req, string? ip, CancellationToken ct);

    /// <summary>Consumes a reset token and sets the new password. Returns null on success, or an error reason.</summary>
    Task<string?> ResetPasswordAsync(ResetPasswordRequest req, CancellationToken ct);
}
