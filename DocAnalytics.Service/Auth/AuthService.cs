using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Auth;

/// <summary>Default <see cref="IAuthService"/> implementation: credential verification, profile, sites, and password change.</summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthService(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    /// <inheritdoc />
    public async Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        // 1) Find user by globally-unique email (safe pre-token lookup)
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive, ct);
        if (user is null) return null;          // null => controller returns 401

        // 2) Verify password against the stored BCrypt hash
        bool passwordOk = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!passwordOk) return null;

        // 3) Which sites can this user see? (the join)
        var sites = await GetSitesForUserAsync(user.Id, ct);

        // 4) Mint the JWT
        var token = _jwt.CreateToken(user);

        return new LoginResponse(
        token,
        new UserDto(user.Id, user.Email, user.Role),
        sites,
        user.MustChangePassword);
    }

    /// <inheritdoc />
    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        if (user is null) return false;

        // must prove they know the current (temporary) password
        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash)) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.MustChangePassword = false;
        await _db.SaveChangesAsync(ct);
        return true;
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
