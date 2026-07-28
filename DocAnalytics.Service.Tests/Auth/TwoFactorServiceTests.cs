using OtpNet;

namespace DocAnalytics.Service.Tests.Auth;

public class TwoFactorServiceTests
{
    private readonly DocAnalytics.Service.Auth.TwoFactorService _sut = new();

    [Fact]
    public void GenerateSetup_ReturnsValidSecret_AndMatchingOtpAuthUri()
    {
        var (secret, uri, manualKey) = _sut.GenerateSetup("user@example.com");

        Assert.NotEmpty(secret);
        Assert.Contains("otpauth://totp/", uri);
        Assert.Contains(secret, uri);
        Assert.Contains("DocAnalytics", uri);
        Assert.Equal(secret, manualKey.Replace(" ", ""));
    }

    [Fact]
    public void GenerateSetup_UsesCustomIssuer_WhenProvided()
    {
        var (_, uri, _) = _sut.GenerateSetup("user@example.com", issuer: "MyCo");

        Assert.Contains("issuer=MyCo", uri);
    }

    [Fact]
    public void ValidateCode_ReturnsTrue_ForCurrentlyValidCode()
    {
        var (secret, _, _) = _sut.GenerateSetup("user@example.com");
        var keyBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(keyBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);

        Assert.True(_sut.ValidateCode(secret, totp.ComputeTotp()));
    }

    [Fact]
    public void ValidateCode_ReturnsFalse_ForWrongCode()
    {
        var (secret, _, _) = _sut.GenerateSetup("user@example.com");

        Assert.False(_sut.ValidateCode(secret, "000000"));
    }

    [Fact]
    public void ValidateCode_ReturnsFalse_ForEmptyOrWhitespaceCode()
    {
        var (secret, _, _) = _sut.GenerateSetup("user@example.com");

        Assert.False(_sut.ValidateCode(secret, ""));
        Assert.False(_sut.ValidateCode(secret, "   "));
    }

    [Fact]
    public void ValidateCode_ReturnsFalse_ForMalformedSecret_NeverThrows()
    {
        Assert.False(_sut.ValidateCode("not-valid-base32!!!", "123456"));
    }

    [Fact]
    public void GenerateRecoveryCodes_ReturnsRequestedCount_InExpectedFormat()
    {
        var codes = _sut.GenerateRecoveryCodes(10);

        Assert.Equal(10, codes.Count);
        Assert.All(codes, c => Assert.Matches("^[A-Z2-9]{4}-[A-Z2-9]{4}$", c));
    }

    [Fact]
    public void GenerateRecoveryCodes_ProducesUniqueCodes()
    {
        var codes = _sut.GenerateRecoveryCodes(20);

        Assert.Equal(codes.Count, codes.Distinct().Count());
    }

    [Fact]
    public void GenerateRecoveryCodes_RespectsCustomCount()
    {
        Assert.Equal(3, _sut.GenerateRecoveryCodes(3).Count);
    }

    [Fact]
    public void HashRecoveryCode_ThenVerify_RoundTrips()
    {
        var hash = _sut.HashRecoveryCode("QJ8F-XFRU");

        Assert.True(_sut.VerifyRecoveryCode("QJ8F-XFRU", hash));
    }

    [Fact]
    public void VerifyRecoveryCode_IsCaseInsensitive_AndTrimsWhitespace()
    {
        var hash = _sut.HashRecoveryCode("QJ8F-XFRU");

        Assert.True(_sut.VerifyRecoveryCode("qj8f-xfru", hash));
        Assert.True(_sut.VerifyRecoveryCode("  QJ8F-XFRU  ", hash));
    }

    [Fact]
    public void VerifyRecoveryCode_ReturnsFalse_ForWrongCode()
    {
        var hash = _sut.HashRecoveryCode("QJ8F-XFRU");

        Assert.False(_sut.VerifyRecoveryCode("WRONG-CODE", hash));
    }
}
