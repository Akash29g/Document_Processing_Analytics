namespace DocAnalytics.Service.Auth;

/// <summary>Validates user-chosen passwords against complexity rules and a breach database.</summary>
public interface IPasswordPolicy
{
    /// <summary>Returns null if acceptable, otherwise a human-readable rejection reason.</summary>
    Task<string?> ValidateAsync(string password, CancellationToken ct = default);
}
