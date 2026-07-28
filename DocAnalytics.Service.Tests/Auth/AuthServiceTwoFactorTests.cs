using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class AuthServiceTwoFactorTests
{
    private readonly Mock<IJwtTokenService> _jwt = new();
    private readonly Mock<IPasswordPolicy> _passwordPolicy = new();
    private readonly Mock<ITwoFactorService> _twoFactor = new();
    private readonly IDataProtectionProvider _dataProtection = new EphemeralDataProtectionProvider();

    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options,
            Mock.Of<ICurrentUser>());


    private AuthService NewSut(AppDbContext db) =>
        new(db, _jwt.Object, _passwordPolicy.Object, _twoFactor.Object, _dataProtection);

    private static User NewUser(AppDbContext db, string password = "Password123!", bool twoFactorEnabled = false)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Admin",
            IsActive = true,
            TwoFactorEnabled = twoFactorEnabled,
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    private string Protect(string plain) =>
        _dataProtection.CreateProtector("DocAnalytics.TwoFactorSecret").Protect(plain);

    [Fact]
    public async Task LoginAsync_ReturnsChallenge_WhenTwoFactorEnabled_DoesNotIssueRealToken()
    {
        using var db = NewDb();
        var user = NewUser(db, "Password123!", twoFactorEnabled: true);
        _jwt.Setup(j => j.CreateTwoFactorChallengeToken(user.Id)).Returns("challenge-token-abc");
        var sut = NewSut(db);

        var result = await sut.LoginAsync(new LoginRequest(user.Email, "Password123!"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result!.RequiresTwoFactor);
        Assert.Equal("challenge-token-abc", result.ChallengeToken);
        Assert.Null(result.Login);
        _jwt.Verify(j => j.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFullLogin_WhenTwoFactorDisabled()
    {
        using var db = NewDb();
        var user = NewUser(db, "Password123!", twoFactorEnabled: false);
        _jwt.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("real-jwt");
        var sut = NewSut(db);

        var result = await sut.LoginAsync(new LoginRequest(user.Email, "Password123!"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result!.RequiresTwoFactor);
        Assert.Equal("real-jwt", result.Login!.Token);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForWrongPassword()
    {
        using var db = NewDb();
        var user = NewUser(db, "Password123!");
        var sut = NewSut(db);

        Assert.Null(await sut.LoginAsync(new LoginRequest(user.Email, "WrongPassword!"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForUnknownEmail()
    {
        using var db = NewDb();
        var sut = NewSut(db);

        Assert.Null(await sut.LoginAsync(new LoginRequest("nobody@test.com", "whatever"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_Succeeds_WithValidTotpCode()
    {
        using var db = NewDb();
        var user = NewUser(db, twoFactorEnabled: true);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();

        _jwt.Setup(j => j.ValidateTwoFactorChallengeToken("valid-token")).Returns(user.Id);
        _jwt.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("real-jwt");
        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "123456")).Returns(true);
        var sut = NewSut(db);

        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("valid-token", "123456"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("real-jwt", result!.Token);
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_FallsBackToRecoveryCode_WhenTotpFails()
    {
        using var db = NewDb();
        var user = NewUser(db, twoFactorEnabled: true);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();
        db.TwoFactorRecoveryCodes.Add(new TwoFactorRecoveryCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = "stored-hash",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        _jwt.Setup(j => j.ValidateTwoFactorChallengeToken("valid-token")).Returns(user.Id);
        _jwt.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("real-jwt");
        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "QJ8F-XFRU")).Returns(false);
        _twoFactor.Setup(t => t.VerifyRecoveryCode("QJ8F-XFRU", "stored-hash")).Returns(true);
        var sut = NewSut(db);

        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("valid-token", "QJ8F-XFRU"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull((await db.TwoFactorRecoveryCodes.SingleAsync()).UsedAt);
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_Fails_WhenRecoveryCodeAlreadyUsed()
    {
        using var db = NewDb();
        var user = NewUser(db, twoFactorEnabled: true);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();
        db.TwoFactorRecoveryCodes.Add(new TwoFactorRecoveryCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = "stored-hash",
            CreatedAt = DateTime.UtcNow,
            UsedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        _jwt.Setup(j => j.ValidateTwoFactorChallengeToken("valid-token")).Returns(user.Id);
        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "QJ8F-XFRU")).Returns(false);
        _twoFactor.Setup(t => t.VerifyRecoveryCode("QJ8F-XFRU", "stored-hash")).Returns(true);
        var sut = NewSut(db);

        var result = await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("valid-token", "QJ8F-XFRU"), CancellationToken.None);

        Assert.Null(result); // already-used codes are excluded from the candidate query entirely
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_ReturnsNull_ForInvalidChallengeToken()
    {
        using var db = NewDb();
        _jwt.Setup(j => j.ValidateTwoFactorChallengeToken("bad-token")).Returns((Guid?)null);
        var sut = NewSut(db);

        Assert.Null(await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("bad-token", "123456"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginWithTwoFactorAsync_ReturnsNull_WhenUserNoLongerHasTwoFactorEnabled()
    {
        using var db = NewDb();
        var user = NewUser(db, twoFactorEnabled: false);
        _jwt.Setup(j => j.ValidateTwoFactorChallengeToken("valid-token")).Returns(user.Id);
        var sut = NewSut(db);

        Assert.Null(await sut.LoginWithTwoFactorAsync(new TwoFactorLoginRequest("valid-token", "123456"), CancellationToken.None));
    }

    [Fact]
    public async Task SetupTwoFactorAsync_StoresEncryptedSecret_AndReturnsSetupPayload()
    {
        using var db = NewDb();
        var user = NewUser(db);
        _twoFactor.Setup(t => t.GenerateSetup(user.Email, "DocAnalytics"))
            .Returns(("SECRETBASE32", "otpauth://totp/...", "SECR ETBA SE32"));
        var sut = NewSut(db);

        var response = await sut.SetupTwoFactorAsync(user.Id, CancellationToken.None);

        Assert.Equal("SECRETBASE32", response.Secret);
        var stored = await db.Users.SingleAsync();
        Assert.NotNull(stored.TwoFactorSecret);
        Assert.NotEqual("SECRETBASE32", stored.TwoFactorSecret); // must be encrypted
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_EnablesTwoFactor_AndIssuesRecoveryCodes_OnValidCode()
    {
        using var db = NewDb();
        var user = NewUser(db);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();

        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "123456")).Returns(true);
        _twoFactor.Setup(t => t.GenerateRecoveryCodes(10)).Returns(new List<string> { "AAAA-1111", "BBBB-2222" });
        _twoFactor.Setup(t => t.HashRecoveryCode(It.IsAny<string>())).Returns("hashed");
        var sut = NewSut(db);

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, "123456", CancellationToken.None);

        Assert.Null(error);
        Assert.Equal(2, result!.RecoveryCodes.Count);
        Assert.True((await db.Users.SingleAsync()).TwoFactorEnabled);
        Assert.Equal(2, await db.TwoFactorRecoveryCodes.CountAsync());
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_ReturnsError_ForInvalidCode()
    {
        using var db = NewDb();
        var user = NewUser(db);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();
        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "000000")).Returns(false);
        var sut = NewSut(db);

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, "000000", CancellationToken.None);

        Assert.NotNull(error);
        Assert.Null(result);
        Assert.False((await db.Users.SingleAsync()).TwoFactorEnabled);
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_ReturnsError_WhenSetupWasNeverCalled()
    {
        using var db = NewDb();
        var user = NewUser(db);
        var sut = NewSut(db);

        var (error, result) = await sut.ConfirmTwoFactorAsync(user.Id, "123456", CancellationToken.None);

        Assert.NotNull(error);
        Assert.Null(result);
    }

    [Fact]
    public async Task ConfirmTwoFactorAsync_WipesStaleRecoveryCodes_OnReConfirm()
    {
        using var db = NewDb();
        var user = NewUser(db);
        user.TwoFactorSecret = Protect("SECRETBASE32");
        await db.SaveChangesAsync();
        db.TwoFactorRecoveryCodes.Add(new TwoFactorRecoveryCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = "old-hash",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        _twoFactor.Setup(t => t.ValidateCode("SECRETBASE32", "123456")).Returns(true);
        _twoFactor.Setup(t => t.GenerateRecoveryCodes(10)).Returns(new List<string> { "NEW1-CODE" });
        _twoFactor.Setup(t => t.HashRecoveryCode(It.IsAny<string>())).Returns("new-hash");
        var sut = NewSut(db);

        await sut.ConfirmTwoFactorAsync(user.Id, "123456", CancellationToken.None);

        var codes = await db.TwoFactorRecoveryCodes.ToListAsync();
        Assert.Single(codes);
        Assert.Equal("new-hash", codes[0].CodeHash);
    }

    [Fact]
    public async Task DisableTwoFactorAsync_ClearsSecretAndRecoveryCodes_OnCorrectPassword()
    {
        using var db = NewDb();
        var user = NewUser(db, "Password123!", twoFactorEnabled: true);
        user.TwoFactorSecret = "encrypted-blob";
        await db.SaveChangesAsync();
        db.TwoFactorRecoveryCodes.Add(new TwoFactorRecoveryCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = "h",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();
        var sut = NewSut(db);

        var error = await sut.DisableTwoFactorAsync(user.Id, "Password123!", CancellationToken.None);

        Assert.Null(error);
        var stored = await db.Users.SingleAsync();
        Assert.False(stored.TwoFactorEnabled);
        Assert.Null(stored.TwoFactorSecret);
        Assert.Empty(await db.TwoFactorRecoveryCodes.ToListAsync());
    }

    [Fact]
    public async Task DisableTwoFactorAsync_ReturnsError_ForWrongPassword()
    {
        using var db = NewDb();
        var user = NewUser(db, "Password123!", twoFactorEnabled: true);
        var sut = NewSut(db);

        var error = await sut.DisableTwoFactorAsync(user.Id, "WrongPassword!", CancellationToken.None);

        Assert.NotNull(error);
        Assert.True((await db.Users.SingleAsync()).TwoFactorEnabled);
    }
}
