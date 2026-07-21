using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;          // IEmailSender
using DocAnalytics.Service.Provisioning;    // ICredentialGenerator
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.AdminUsers;

/// <summary>Default <see cref="IAdminUserService"/> implementation; all queries are explicitly scoped to the caller's tenant.</summary>
// Every query here is EXPLICITLY scoped to _currentUser.TenantId —
// Users/Sites are not ITenantScoped, so the global filter doesn't cover them.
public sealed class AdminUserService : IAdminUserService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly ICredentialGenerator _credentials;
    private readonly IEmailSender _email;

    public AdminUserService(AppDbContext db, ICurrentUser currentUser,
        ICredentialGenerator credentials, IEmailSender email)
    {
        _db = db; _currentUser = currentUser;
        _credentials = credentials; _email = email;
    }

    /// <inheritdoc />
    public async Task<List<AdminUserDto>> GetUsersAsync(CancellationToken ct) =>
        await _db.Users.AsNoTracking()
            .Where(u => u.TenantId == _currentUser.TenantId && u.Role == "Viewer")
            .OrderBy(u => u.Email)
            .Select(u => new AdminUserDto(
                u.Id, u.Email, u.Role, u.IsActive, u.CreatedAt,
                u.SiteAccess.Select(a => a.SiteId).ToList()))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<AdminCreatedUserDto?> CreateUserAsync(AdminCreateUserRequest req, CancellationToken ct)
    {
        var tenant = await _db.Tenants.AsNoTracking()
            .FirstAsync(t => t.Id == _currentUser.TenantId, ct);

        // every requested site must belong to MY tenant and be active
        var validSiteIds = await _db.Sites.AsNoTracking()
            .Where(s => s.TenantId == _currentUser.TenantId && s.IsActive && req.SiteIds.Contains(s.Id))
            .Select(s => s.Id).ToListAsync(ct);
        if (validSiteIds.Count != req.SiteIds.Distinct().Count())
            return null;   // controller → 400 INVALID_SITES

        var taken = (await _db.Users.AsNoTracking().Select(u => u.Email).ToListAsync(ct))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var email = _credentials.BuildEmail(req.FirstName, req.LastName, tenant.OrgDomain, taken);
        var password = _credentials.GeneratePassword();

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _currentUser.TenantId,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Viewer",
            MustChangePassword = true,
            IsActive = true,
            CreatedBy = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        _db.UserSiteAccess.AddRange(validSiteIds.Select(sid => new UserSiteAccess
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SiteId = sid,
            GrantedAt = DateTime.UtcNow
        }));
        await _db.SaveChangesAsync(ct);

        await _email.SendAsync(email, $"Your {tenant.Name} account",
            $"Hello {req.FirstName},\n\nYour account has been created.\n" +
            $"Login: {email}\nTemporary password: {password}\n\n" +
            "You will be asked to change your password on first login.", ct);

        return new AdminCreatedUserDto(user.Id, user.Email, CredentialsEmailed: true);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateUserSitesAsync(Guid userId, UpdateUserSitesRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Id == userId && u.TenantId == _currentUser.TenantId && u.Role == "Viewer" && u.IsActive, ct);
        if (user is null) return false;

        var validSiteIds = await _db.Sites.AsNoTracking()
            .Where(s => s.TenantId == _currentUser.TenantId && s.IsActive && req.SiteIds.Contains(s.Id))
            .Select(s => s.Id).ToListAsync(ct);
        if (validSiteIds.Count != req.SiteIds.Distinct().Count()) return false;

        var existing = await _db.UserSiteAccess.Where(a => a.UserId == userId).ToListAsync(ct);
        _db.UserSiteAccess.RemoveRange(existing.Where(a => !validSiteIds.Contains(a.SiteId)));
        _db.UserSiteAccess.AddRange(validSiteIds
            .Where(sid => !existing.Any(a => a.SiteId == sid))
            .Select(sid => new UserSiteAccess
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SiteId = sid,
                GrantedAt = DateTime.UtcNow
            }));
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Id == userId && u.TenantId == _currentUser.TenantId && u.Role == "Viewer" && u.IsActive, ct);
        if (user is null) return false;   // can't touch Admins or other tenants

        user.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<AdminSiteDto>> GetSitesAsync(CancellationToken ct) =>
        await _db.Sites.AsNoTracking()
            .Where(s => s.TenantId == _currentUser.TenantId)
            .OrderBy(s => s.Name)
            .Select(s => new AdminSiteDto(s.Id, s.Name, s.Location, s.IsActive))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<AdminSiteDto> CreateSiteAsync(AdminCreateSiteRequest req, CancellationToken ct)
    {
        var site = new Site
        {
            Id = Guid.NewGuid(),
            TenantId = _currentUser.TenantId,
            Name = req.Name.Trim(),
            Location = req.Location?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Sites.Add(site);

        // creating admin automatically gets access to the new site
        _db.UserSiteAccess.Add(new UserSiteAccess
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.UserId,
            SiteId = site.Id,
            GrantedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
        return new AdminSiteDto(site.Id, site.Name, site.Location, true);
    }
}
