using System.Security.Cryptography;
using System.Text;

namespace DocAnalytics.Service.Auth;

public sealed class PasswordPolicy : IPasswordPolicy
{
    private readonly HttpClient _http;
    public PasswordPolicy(HttpClient http) => _http = http;

    public async Task<string?> ValidateAsync(string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            return "Password must be at least 12 characters.";
        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) ||
            !password.Any(char.IsDigit) || password.All(char.IsLetterOrDigit))
            return "Password must include upper, lower, a digit, and a symbol.";

        if (await IsBreachedAsync(password, ct))
            return "This password has appeared in a known data breach. Choose another.";

        return null;
    }

    // k-anonymity: send only the first 5 chars of the SHA-1 hash, never the password.
    private async Task<bool> IsBreachedAsync(string password, CancellationToken ct)
    {
        var hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(password)));
        var prefix = hash[..5];
        var suffix = hash[5..];

        try
        {
            var resp = await _http.GetStringAsync($"https://api.pwnedpasswords.com/range/{prefix}", ct);
            return resp.Split('\n').Any(line => line.StartsWith(suffix, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false; // fail-open on HIBP outage — don't block on a 3rd-party blip
        }
    }
}
