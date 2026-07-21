using System.Security.Cryptography;

namespace DocAnalytics.Service.Provisioning;

/// <summary>Generates secure passwords and unique login emails for newly provisioned users.</summary>
public interface ICredentialGenerator
{
    /// <summary>Generates a cryptographically random password that includes at least one upper, lower, digit, and symbol.</summary>
    /// <param name="length">The desired password length (minimum effective length is 4).</param>
    /// <returns>The generated password.</returns>
    string GeneratePassword(int length = 14);

    /// <summary>Builds a unique <c>first.last@domain</c> email, appending a numeric suffix if the address is already taken.</summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="orgDomain">The organization domain.</param>
    /// <param name="takenEmails">The set of already-used emails to avoid.</param>
    /// <returns>A unique email address.</returns>
    string BuildEmail(string firstName, string lastName, string orgDomain, ISet<string> takenEmails);
}

/// <summary>Default <see cref="ICredentialGenerator"/> implementation using a crypto RNG and ambiguous-character-free alphabets.</summary>
public sealed class CredentialGenerator : ICredentialGenerator
{
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnpqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!@#$%^&*";

    /// <inheritdoc />
    public string GeneratePassword(int length = 14)
    {
        var all = Upper + Lower + Digits + Symbols;
        var chars = new List<char>
        {
            Pick(Upper), Pick(Lower), Pick(Digits), Pick(Symbols)   // guarantee each class
        };
        while (chars.Count < length) chars.Add(Pick(all));
        // Fisher–Yates shuffle with crypto RNG
        for (int i = chars.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars.ToArray());
    }

    /// <inheritdoc />
    public string BuildEmail(string firstName, string lastName, string orgDomain, ISet<string> takenEmails)
    {
        var local = $"{Sanitize(firstName)}.{Sanitize(lastName)}";
        var email = $"{local}@{orgDomain}";
        int n = 2;
        while (takenEmails.Contains(email))
            email = $"{local}{n++}@{orgDomain}";
        return email;
    }

    private static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];

    private static string Sanitize(string s) =>
        new(s.Trim().ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
}
