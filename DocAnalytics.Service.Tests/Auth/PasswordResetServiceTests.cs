using System.Security.Cryptography;
using System.Text;
using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;
using DocAnalytics.Service.Auth;
using Microsoft.Extensions.Configuration;
using DocAnalytics.Service.Tests.Support;
using Moq;

namespace DocAnalytics.Service.Tests.Auth;

public class PasswordResetServiceTests
{
    private readonly Mock<IPasswordPolicy> _policy = new();   // default ValidateAsync => null (valid)
    private readonly Mock<IEmailSender> _email = new();

    private PasswordResetService Sut(AppDbContext db)
    {
        var cfg = new Mock<IConfiguration>();
        cfg.Setup(c => c["App:FrontendBaseUrl"]).Returns((string?)null);   // falls back to localhost
        return new PasswordResetService(db, _policy.Object, _email.Object, cfg.Object);
    }

    private static User ActiveUser(string email) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        Email = email,
        PasswordHash = "OLD-HASH",
        Role = "Viewer",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    // Mirrors the service's private hashing so we can craft a known token for reset tests.
    private static string Hash(string raw)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

    // ---- RequestResetAsync ----

    [Fact]
    public async Task RequestResetAsync_creates_token_and_sends_email()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        db.Users.Add(ActiveUser("rita@acme.com"));
        db.SaveChanges();

        await Sut(db).RequestResetAsync(new ForgotPasswordRequest("rita@acme.com"), "1.2.3.4", default);

        var token = Assert.Single(db.PasswordResetTokens);
        Assert.False(string.IsNullOrWhiteSpace(token.TokenHash));
        Assert.True(token.ExpiresAt > DateTime.UtcNow);
        Assert.Null(token.UsedAt);
        _email.Verify(e => e.SendAsync("rita@acme.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestResetAsync_unknown_email_does_nothing()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());

        await Sut(db).RequestResetAsync(new ForgotPasswordRequest("nobody@x.com"), null, default);

        Assert.Empty(db.PasswordResetTokens);
        _email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RequestResetAsync_invalidates_previous_unused_tokens()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("old-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        await Sut(db).RequestResetAsync(new ForgotPasswordRequest("rita@acme.com"), null, default);

        Assert.Equal(2, db.PasswordResetTokens.Count());                       // old + new
        Assert.Equal(1, db.PasswordResetTokens.Count(t => t.UsedAt == null));  // only one live
    }

    // ---- ResetPasswordAsync ----

    [Fact]
    public async Task ResetPasswordAsync_sets_new_password_and_consumes_token()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        user.MustChangePassword = true;
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("raw-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        var error = await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("raw-token", "NewPass1!"), default);

        Assert.Null(error);
        var savedUser = db.Users.Single(u => u.Id == user.Id);
        Assert.NotEqual("OLD-HASH", savedUser.PasswordHash);   // password changed
        Assert.False(savedUser.MustChangePassword);
        Assert.NotNull(db.PasswordResetTokens.Single().UsedAt); // token consumed
    }

    [Fact]
    public async Task ResetPasswordAsync_returns_error_for_unknown_token()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());

        var error = await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("does-not-exist", "NewPass1!"), default);

        Assert.Equal("Invalid or expired reset link.", error);
    }

    [Fact]
    public async Task ResetPasswordAsync_returns_error_for_expired_token()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("raw-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1),   // expired
            CreatedAt = DateTime.UtcNow.AddMinutes(-31)
        });
        db.SaveChanges();

        var error = await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("raw-token", "NewPass1!"), default);

        Assert.Equal("Invalid or expired reset link.", error);
    }

    [Fact]
    public async Task ResetPasswordAsync_returns_error_for_used_token()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("raw-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow,
            UsedAt = DateTime.UtcNow                        // already consumed
        });
        db.SaveChanges();

        var error = await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("raw-token", "NewPass1!"), default);

        Assert.Equal("Invalid or expired reset link.", error);
    }

    [Fact]
    public async Task ResetPasswordAsync_rejects_weak_password_without_consuming_token()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("raw-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        _policy.Setup(p => p.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync("Password is too weak.");

        var error = await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("raw-token", "weak"), default);

        Assert.Equal("Password is too weak.", error);
        Assert.Equal("OLD-HASH", db.Users.Single().PasswordHash);       // unchanged
        Assert.Null(db.PasswordResetTokens.Single().UsedAt);            // NOT consumed
    }

    [Fact]
    public async Task ResetPasswordAsync_revokes_active_refresh_tokens()
    {
        using var db = InMemoryDb.Create(new TestCurrentUser());
        var user = ActiveUser("rita@acme.com");
        db.Users.Add(user);
        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash("raw-token"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        });
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "refresh-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        await Sut(db).ResetPasswordAsync(new ResetPasswordRequest("raw-token", "NewPass1!"), default);

        Assert.NotNull(db.RefreshTokens.Single().RevokedAt);   // old sessions killed
    }
}
