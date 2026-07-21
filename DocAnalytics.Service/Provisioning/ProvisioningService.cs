using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Alerts;   // IEmailSender
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Provisioning;

/// <summary>Default <see cref="IProvisioningService"/> implementation: super-admin tenant, site, and admin-user management.</summary>
public sealed class ProvisioningService : IProvisioningService
{
    private readonly AppDbContext _db;
    private readonly ICredentialGenerator _credentials;
    private readonly IEmailSender _email;

    public ProvisioningService(AppDbContext db, ICredentialGenerator credentials, IEmailSender email)
    {
        _db = db; _credentials = credentials; _email = email;
    }

    // NOTE: Tenants/Users/Sites are NOT ITenantScoped → no global filter applies.
    // Only counts and identity fields are exposed — never business data.

    /// <inheritdoc />
    public async Task<List<TenantSummaryDto>> GetTenantsAsync(CancellationToken ct) =>
        await _db.Tenants.AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TenantSummaryDto(
                t.Id, t.Name, t.OrgDomain, t.IsActive,
                t.Sites.Count(s => s.IsActive),
                t.Users.Count(u => u.IsActive),
                t.Users.Count(u => u.IsActive && u.Role == "Admin")))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<TenantSummaryDto?> CreateTenantAsync(CreateTenantRequest req, CancellationToken ct)
    {
        var domain = req.OrgDomain.Trim().ToLowerInvariant();
        if (await _db.Tenants.AnyAsync(t => t.OrgDomain == domain, ct))
            return null;   // controller → 409

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            OrgDomain = domain,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);
        return new TenantSummaryDto(tenant.Id, tenant.Name, tenant.OrgDomain, true, 0, 0, 0);
    }

    /// <inheritdoc />
    public async Task<List<ProvisionedUserDto>> GetUsersAsync(Guid tenantId, CancellationToken ct) =>
        await _db.Users.AsNoTracking()
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Email)
            .Select(u => new ProvisionedUserDto(u.Id, u.Email, u.Role, u.IsActive, u.CreatedAt))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<ProvisionedUserDto?> CreateAdminAsync(
        Guid tenantId, CreateAdminRequest req, Guid createdBy, CancellationToken ct)
    {
        var tenant = await _db.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive, ct);
        if (tenant is null) return null;   // controller → 404

        var taken = (await _db.Users.AsNoTracking().Select(u => u.Email).ToListAsync(ct))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var email = _credentials.BuildEmail(req.FirstName, req.LastName, tenant.OrgDomain, taken);
        var password = _credentials.GeneratePassword();

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Admin",
            MustChangePassword = true,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // Admin gets access to ALL current sites of the tenant (they manage them anyway)
        var siteIds = await _db.Sites.AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.IsActive)
            .Select(s => s.Id).ToListAsync(ct);
        if (siteIds.Count > 0)
        {
            _db.UserSiteAccess.AddRange(siteIds.Select(sid => new UserSiteAccess
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SiteId = sid,
                GrantedAt = DateTime.UtcNow
            }));
            await _db.SaveChangesAsync(ct);
        }

        await _email.SendAsync(email, $"Your {tenant.Name} admin account",
            $"Hello {req.FirstName},\n\nYour admin account has been created.\n" +
            $"Login: {email}\nTemporary password: {password}\n\n" +
            "You will be asked to change your password on first login.", ct);

        return new ProvisionedUserDto(user.Id, user.Email, user.Role, true, user.CreatedAt);
    }

    /// <inheritdoc />
    public Task<bool> RemoveAdminAsync(Guid tenantId, Guid userId, CancellationToken ct) =>
        DeactivateUserAsync(tenantId, userId, requiredRole: "Admin", ct);

    /// <inheritdoc />
    public Task<bool> RemoveUserAsync(Guid tenantId, Guid userId, CancellationToken ct) =>
        DeactivateUserAsync(tenantId, userId, requiredRole: null, ct);

    private async Task<bool> DeactivateUserAsync(Guid tenantId, Guid userId, string? requiredRole, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Id == userId && u.TenantId == tenantId && u.IsActive, ct);
        if (user is null) return false;
        if (requiredRole is not null && user.Role != requiredRole) return false;

        user.IsActive = false;   // soft delete — audit rows keep their FK
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<ProvisionedSiteDto>> GetSitesAsync(Guid tenantId, CancellationToken ct) =>
        await _db.Sites.AsNoTracking()
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Name)
            .Select(s => new ProvisionedSiteDto(s.Id, s.Name, s.Location, s.IsActive))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<ProvisionedSiteDto?> CreateSiteAsync(Guid tenantId, CreateSiteRequest req, CancellationToken ct)
    {
        if (!await _db.Tenants.AnyAsync(t => t.Id == tenantId && t.IsActive, ct))
            return null;

        var site = new Site
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = req.Name.Trim(),
            Location = req.Location?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Sites.Add(site);
        await _db.SaveChangesAsync(ct);
        return new ProvisionedSiteDto(site.Id, site.Name, site.Location, true);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveSiteAsync(Guid tenantId, Guid siteId, CancellationToken ct)
    {
        var site = await _db.Sites.FirstOrDefaultAsync(
            s => s.Id == siteId && s.TenantId == tenantId && s.IsActive, ct);
        if (site is null) return false;

        site.IsActive = false;   // soft delete
        // revoke everyone's access rows for this site
        var access = await _db.UserSiteAccess.Where(a => a.SiteId == siteId).ToListAsync(ct);
        _db.UserSiteAccess.RemoveRange(access);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
