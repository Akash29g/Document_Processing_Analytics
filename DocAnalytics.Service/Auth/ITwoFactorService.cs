namespace DocAnalytics.Service.Auth;

/// <summary>TOTP secret generation/validation and recovery-code generation/verification for 2FA.</summary>
public interface ITwoFactorService
{
    /// <summary>Generates a new Base32 secret + otpauth:// URI (for client-side QR rendering) + a spaced manual-entry key.</summary>
    (string Secret, string OtpAuthUri, string ManualKey) GenerateSetup(string accountLabel, string issuer = "DocAnalytics");

    /// <summary>Validates a 6-digit TOTP code against a Base32 secret, tolerating ±1 time-step clock drift.</summary>
    bool ValidateCode(string base32Secret, string code);

    /// <summary>Generates single-use recovery codes in "XXXX-XXXX" form (plaintext — caller shows once, never persists raw).</summary>
    IReadOnlyList<string> GenerateRecoveryCodes(int count = 10);

    /// <summary>BCrypt-hashes a recovery code for storage.</summary>
    string HashRecoveryCode(string code);

    /// <summary>Verifies a presented recovery code against its stored BCrypt hash.</summary>
    bool VerifyRecoveryCode(string code, string hash);
}
