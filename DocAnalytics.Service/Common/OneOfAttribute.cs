using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Common;

// Reusable whitelist check: value must be one of the allowed strings (case-insensitive).
// Null/blank passes — use it on OPTIONAL fields. Combine with [Required] if mandatory.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OneOfAttribute : ValidationAttribute
{
    private readonly string[] _allowed;

    public OneOfAttribute(params string[] allowed) => _allowed = allowed;

    public override bool IsValid(object? value)
    {
        var s = value as string;
        if (string.IsNullOrWhiteSpace(s)) return true;   // not supplied → nothing to validate

        return _allowed.Any(a => string.Equals(a, s, StringComparison.OrdinalIgnoreCase));
    }

    public override string FormatErrorMessage(string name)
        => $"'{name}' must be one of: {string.Join(", ", _allowed)}.";
}
