using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Alerts;

/// <summary>Default <see cref="IAlertNotificationService"/> implementation; auto-scoped to the request's tenant/site.</summary>
public sealed class AlertNotificationService : IAlertNotificationService
{
    private readonly AppDbContext _db;
    public AlertNotificationService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    // Global query filter auto-scopes to the request's tenant+site (NFR-3).
    public async Task<List<AlertNotificationDto>> GetNotificationsAsync(
        bool unreadOnly, CancellationToken ct = default)
    {
        var q = _db.AlertNotifications.AsNoTracking();
        if (unreadOnly) q = q.Where(n => !n.IsRead);

        return await q
            .OrderByDescending(n => n.FiredAt)
            .ThenByDescending(n => n.Id)              // stable order on tied timestamps
            .Take(50)                                 // login burst cap — never flood the UI
            .Select(n => new AlertNotificationDto
            {
                Id = n.Id,
                AlertRuleId = n.AlertRuleId,
                RuleName = n.RuleName,
                Message = n.Message,
                Severity = n.Severity,
                ObservedPercent = n.ObservedPercent,
                ThresholdPercent = n.ThresholdPercent,
                IsRead = n.IsRead,
                FiredAt = n.FiredAt,
                ReadAt = n.ReadAt
            })
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public Task<int> GetUnreadCountAsync(CancellationToken ct = default) =>
        _db.AlertNotifications.CountAsync(n => !n.IsRead, ct);

    /// <inheritdoc />
    public async Task<bool> MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        // Tracked query → filter still applies, so cross-tenant ids just return null → false.
        var row = await _db.AlertNotifications.FirstOrDefaultAsync(n => n.Id == id, ct);
        if (row is null) return false;
        if (row.IsRead) return true;

        row.IsRead = true;
        row.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<int> MarkAllReadAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var unread = await _db.AlertNotifications.Where(n => !n.IsRead).ToListAsync(ct);
        foreach (var n in unread) { n.IsRead = true; n.ReadAt = now; }
        await _db.SaveChangesAsync(ct);
        return unread.Count;
    }
}
