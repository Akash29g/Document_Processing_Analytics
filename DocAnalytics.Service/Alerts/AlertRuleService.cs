using DocAnalytics.Data;                 // AppDbContext
using DocAnalytics.Domain.Common;        // ICurrentUser
using DocAnalytics.Domain.Entities;      // AlertRule
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Alerts;

public sealed class AlertRuleService : IAlertRuleService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;   // set by TenantSiteMiddleware

    public AlertRuleService(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    // reads auto-scope to tenant/site via the global ITenantScoped filter
    public async Task<IReadOnlyList<AlertRuleDto>> ListAsync(CancellationToken ct = default) =>
        await _db.AlertRules.AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => ToDto(r))
            .ToListAsync(ct);

    public async Task<AlertRuleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var r = await _db.AlertRules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return r is null ? null : ToDto(r);
    }

    public async Task<AlertRuleDto> CreateAsync(CreateAlertRuleRequest req, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var rule = new AlertRule
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,     // stamp current tenant/site
            SiteId = _me.SiteId,
            Name = req.Name.Trim(),
            ThresholdPercent = req.ThresholdPercent,
            WindowMinutes = req.WindowMinutes <= 0 ? 60 : req.WindowMinutes,
            Email = req.Email.Trim(),
            CooldownMinutes = req.CooldownMinutes <= 0 ? 60 : req.CooldownMinutes,
            IsEnabled = req.IsEnabled,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.AlertRules.Add(rule);
        await _db.SaveChangesAsync(ct);
        return ToDto(rule);
    }

    public async Task<AlertRuleDto?> UpdateAsync(Guid id, UpdateAlertRuleRequest req, CancellationToken ct = default)
    {
        var rule = await _db.AlertRules.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (rule is null) return null;

        rule.Name = req.Name.Trim();
        rule.ThresholdPercent = req.ThresholdPercent;
        rule.WindowMinutes = req.WindowMinutes <= 0 ? 60 : req.WindowMinutes;
        rule.Email = req.Email.Trim();
        rule.CooldownMinutes = req.CooldownMinutes <= 0 ? 60 : req.CooldownMinutes;
        rule.IsEnabled = req.IsEnabled;
        rule.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return ToDto(rule);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rule = await _db.AlertRules.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (rule is null) return false;
        _db.AlertRules.Remove(rule);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static AlertRuleDto ToDto(AlertRule r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        ThresholdPercent = r.ThresholdPercent,
        WindowMinutes = r.WindowMinutes,
        Email = r.Email,
        IsEnabled = r.IsEnabled,
        CooldownMinutes = r.CooldownMinutes,
        LastTriggeredAt = r.LastTriggeredAt,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
