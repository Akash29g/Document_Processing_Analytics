using DocAnalytics.Service.Provisioning;

namespace DocAnalytics.Service.Tests.Provisioning;

public class CredentialGeneratorTests
{
    private readonly CredentialGenerator _gen = new();

    [Fact]
    public void GeneratePassword_has_requested_length_and_all_char_classes()
    {
        var pw = _gen.GeneratePassword(14);

        Assert.Equal(14, pw.Length);
        Assert.Contains(pw, char.IsUpper);
        Assert.Contains(pw, char.IsLower);
        Assert.Contains(pw, char.IsDigit);
        Assert.Contains(pw, c => "!@#$%^&*".Contains(c));
    }

    [Fact]
    public void GeneratePassword_is_not_deterministic()
    {
        Assert.NotEqual(_gen.GeneratePassword(), _gen.GeneratePassword());
    }

    [Fact]
    public void BuildEmail_formats_first_dot_last_at_domain()
    {
        var email = _gen.BuildEmail("Rita", "Sharma", "acme.com", new HashSet<string>());
        Assert.Equal("rita.sharma@acme.com", email);
    }

    [Fact]
    public void BuildEmail_appends_number_when_taken()
    {
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "rita.sharma@acme.com", "rita.sharma2@acme.com" };

        var email = _gen.BuildEmail("Rita", "Sharma", "acme.com", taken);
        Assert.Equal("rita.sharma3@acme.com", email);
    }

    [Fact]
    public void BuildEmail_strips_spaces_and_special_chars()
    {
        var email = _gen.BuildEmail("  Ri ta ", "O'Brien", "acme.com", new HashSet<string>());
        Assert.Equal("rita.obrien@acme.com", email);
    }
}
