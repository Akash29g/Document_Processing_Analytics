using DocAnalytics.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Auth;

/// <summary>Default <see cref="IAuthService"/> implementation: credential verification, profile, sites, and password change.</summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly ITwoFactorService _twoFactor;
    private readonly Microsoft.AspNetCore.DataProtection.IDataProtector _protector;

    public AuthService(
        AppDbContext db,
        IJwtTokenService jwt,
        IPasswordPolicy passwordPolicy,
        ITwoFactorService twoFactor,
        Microsoft.AspNetCore.DataProtection.IDataProtectionProvider dataProtection)
    {
        _db = db;
        _jwt = jwt;
        _passwordPolicy = passwordPolicy;
        _twoFactor = twoFactor;
        _protector = dataProtection.CreateProtector("DocAnalytics.TwoFactorSecret");
    }


    /// <inheritdoc />
    public async Task<LoginResult?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        // 1) Find user by globally-unique email (safe pre-token lookup)
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive, ct);
        if (user is null) return null;          // null => controller returns 401

        // 2) Verify password against the stored BCrypt hash
        bool passwordOk = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!passwordOk) return null;

        // 2fa) Do NOT issue the real access token yet — only a short-lived, purpose-scoped challenge token.
        if (user.TwoFactorEnabled)
        {
            var challengeToken = _jwt.CreateTwoFactorChallengeToken(user.Id);
            return new LoginResult(true, challengeToken, null);
        }

        // 3) Which sites can this user see? (the join)
        var sites = await GetSitesForUserAsync(user.Id, ct);

        // 4) Mint the JWT
        var token = _jwt.CreateToken(user);

        var login = new LoginResponse(
            token,
            new UserDto(user.Id, user.Email, user.Role),
            sites,
            user.MustChangePassword);

        return new LoginResult(false, null, login);
    }

    /// <inheritdoc />
    public async Task<LoginResponse?> LoginWithTwoFactorAsync(TwoFactorLoginRequest req, CancellationToken ct)
    {
        var userId = _jwt.ValidateTwoFactorChallengeToken(req.ChallengeToken);
        if (userId is null) return null;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        if (user is null || !user.TwoFactorEnabled || user.TwoFactorSecret is null) return null;

        var secret = _protector.Unprotect(user.TwoFactorSecret);
        bool codeOk = _twoFactor.ValidateCode(secret, req.Code);

        if (!codeOk)
        {
            codeOk = await TryConsumeRecoveryCodeAsync(user.Id, req.Code, ct); // fall back to a single-use recovery code
        }
        if (!codeOk) return null;

        var sites = await GetSitesForUserAsync(user.Id, ct);
        var token = _jwt.CreateToken(user);
        return new LoginResponse(token, new UserDto(user.Id, user.Email, user.Role), sites, user.MustChangePassword);
    }

    /// <inheritdoc />
    public async Task<TwoFactorSetupResponse> SetupTwoFactorAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FirstAsync(u => u.Id == userId, ct);
        var (secret, uri, manualKey) = _twoFactor.GenerateSetup(user.Email);

        // Store encrypted immediately so /confirm can validate against it; NOT enabled until confirmed.
        user.TwoFactorSecret = _protector.Protect(secret);
        await _db.SaveChangesAsync(ct);

        return new TwoFactorSetupResponse(secret, uri, manualKey);
    }

    /// <inheritdoc />
    public async Task<(string? Error, TwoFactorConfirmResponse? Result)> ConfirmTwoFactorAsync(Guid userId, string code, CancellationToken ct)
    {
        var user = await _db.Users.FirstAsync(u => u.Id == userId, ct);
        if (user.TwoFactorSecret is null) return ("Call /auth/2fa/setup first.", null);

        var secret = _protector.Unprotect(user.TwoFactorSecret);
        if (!_twoFactor.ValidateCode(secret, code)) return ("Invalid code. Check your app and try again.", null);

        user.TwoFactorEnabled = true;

        var old = await _db.TwoFactorRecoveryCodes.Where(r => r.UserId == userId).ToListAsync(ct);
        _db.TwoFactorRecoveryCodes.RemoveRange(old); // re-confirm scenario — wipe stale codes

        var plainCodes = _twoFactor.GenerateRecoveryCodes();
        foreach (var plain in plainCodes)
        {
            _db.TwoFactorRecoveryCodes.Add(new DocAnalytics.Domain.Entities.TwoFactorRecoveryCode
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CodeHash = _twoFactor.HashRecoveryCode(plain),
                CreatedAt = DateTime.UtcNow,
            });
        }
        await _db.SaveChangesAsync(ct);

        return (null, new TwoFactorConfirmResponse(plainCodes));
    }

    /// <inheritdoc />
    public async Task<string?> DisableTwoFactorAsync(Guid userId, string password, CancellationToken ct)
    {
        var user = await _db.Users.FirstAsync(u => u.Id == userId, ct);
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return "Password is incorrect.";

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;

        var codes = await _db.TwoFactorRecoveryCodes.Where(r => r.UserId == userId).ToListAsync(ct);
        _db.TwoFactorRecoveryCodes.RemoveRange(codes);

        await _db.SaveChangesAsync(ct);
        return null;
    }

    private async Task<bool> TryConsumeRecoveryCodeAsync(Guid userId, string presented, CancellationToken ct)
    {
        var candidates = await _db.TwoFactorRecoveryCodes
            .Where(r => r.UserId == userId && r.UsedAt == null)
            .ToListAsync(ct);

        var match = candidates.FirstOrDefault(c => _twoFactor.VerifyRecoveryCode(presented, c.CodeHash));
        if (match is null) return false;

        match.UsedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }


    /// <inheritdoc />
    public async Task<string?> ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        if (user is null) return "User not found.";
        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash))
            return "Current password is incorrect.";

        var reason = await _passwordPolicy.ValidateAsync(req.NewPassword, ct);
        if (reason is not null) return reason;   // e.g. "This password has appeared in a known data breach."

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.MustChangePassword = false;
        await _db.SaveChangesAsync(ct);
        return null;   // success

    }


    /// <inheritdoc />
    public async Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        if (user is null) return null;

        var sites = await GetSitesForUserAsync(userId, ct);
        return new MeResponse(new UserDto(user.Id, user.Email, user.Role), sites);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SiteDto>> GetSitesAsync(Guid userId, CancellationToken ct)
        => GetSitesForUserAsync(userId, ct);

    // The UserSiteAccess → Sites join, written once, reused by all three endpoints
    private async Task<IReadOnlyList<SiteDto>> GetSitesForUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserSiteAccess
            .Where(usa => usa.UserId == userId)
            .Join(
                _db.Sites.Where(s => s.IsActive),
                usa => usa.SiteId,
                s => s.Id,
                (usa, s) => new SiteDto(s.Id, s.Name))
            .ToListAsync(ct);
    }
}
