using System.Security.Cryptography;
using OtpNet;

namespace DocAnalytics.Service.Auth;

/// <summary>Default <see cref="ITwoFactorService"/> implementation, built on Otp.NET.</summary>
public sealed class TwoFactorService : ITwoFactorService
{
    /// <inheritdoc />
    public (string Secret, string OtpAuthUri, string ManualKey) GenerateSetup(string accountLabel, string issuer = "DocAnalytics")
    {
        var keyBytes = KeyGeneration.GenerateRandomKey(20); // 160-bit — standard TOTP secret size
        var secret = Base32Encoding.ToString(keyBytes);

        var label = Uri.EscapeDataString($"{issuer}:{accountLabel}");
        var uri = $"otpauth://totp/{label}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&digits=6&period=30&algorithm=SHA1";

        var manualKey = string.Join(' ', Chunk(secret, 4)); // e.g. "ABCD EFGH IJKL ..."

        return (secret, uri, manualKey);
    }

    /// <inheritdoc />
    public bool ValidateCode(string base32Secret, string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        try
        {
            var keyBytes = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(keyBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(code.Trim(), out _, new VerificationWindow(previous: 1, future: 1));
        }
        catch
        {
            return false; // malformed secret/code — never throw out of a security check
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GenerateRecoveryCodes(int count = 10)
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no O/0/I/1 ambiguity
        var codes = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var chars = new char[8];
            for (int j = 0; j < 8; j++) chars[j] = alphabet[bytes[j] % alphabet.Length];
            codes.Add($"{new string(chars, 0, 4)}-{new string(chars, 4, 4)}");
        }
        return codes;
    }

    /// <inheritdoc />
    public string HashRecoveryCode(string code) => BCrypt.Net.BCrypt.HashPassword(Normalize(code));

    /// <inheritdoc />
    public bool VerifyRecoveryCode(string code, string hash) => BCrypt.Net.BCrypt.Verify(Normalize(code), hash);

    private static string Normalize(string code) => code.Trim().ToUpperInvariant();

    private static IEnumerable<string> Chunk(string s, int size)
    {
        for (int i = 0; i < s.Length; i += size)
            yield return s.Substring(i, Math.Min(size, s.Length - i));
    }
}
