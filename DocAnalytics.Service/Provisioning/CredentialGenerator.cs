using System.Security.Cryptography;

namespace DocAnalytics.Service.Provisioning;

public interface ICredentialGenerator
{
    string GeneratePassword(int length = 14);
    string BuildEmail(string firstName, string lastName, string orgDomain, ISet<string> takenEmails);
}

public sealed class CredentialGenerator : ICredentialGenerator
{
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnpqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!@#$%^&*";

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
