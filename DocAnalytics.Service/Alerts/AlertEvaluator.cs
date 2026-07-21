using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocAnalytics.Service.Alerts;

/// <summary>Default <see cref="IAlertEvaluator"/> implementation; scans all sites and fires alerts that exceed their threshold.</summary>
public sealed class AlertEvaluator : IAlertEvaluator
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _email;
    private readonly ILogger<AlertEvaluator> _logger;

    public AlertEvaluator(AppDbContext db, IEmailSender email, ILogger<AlertEvaluator> logger)
    {
        _db = db;
        _email = email;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task EvaluateAllAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        // Runs OUTSIDE an HTTP request → no ICurrentUser set → global tenant filter would
        // hide everything. IgnoreQueryFilters() lets us scan ALL sites; we scope per-rule below.
        var rules = await _db.AlertRules
            .IgnoreQueryFilters()
            .Where(r => r.IsEnabled)
            .ToListAsync(ct);   // tracked → we update LastTriggeredAt

        foreach (var rule in rules)
        {
            var since = now.AddMinutes(-rule.WindowMinutes);

            var files = _db.Files
                .IgnoreQueryFilters()
                .Where(f => f.SiteId == rule.SiteId && f.LastUpdatedAt >= since);

            var total = await files.CountAsync(ct);
            if (total == 0) continue;                       // nothing happened in the window

            var failed = await files.CountAsync(f => f.Status == "Failed", ct);
            var rate = 100.0 * failed / total;
            if (rate <= rule.ThresholdPercent) continue;    // under threshold → OK

            // cooldown so we don't spam the same alert
            if (rule.LastTriggeredAt.HasValue &&
                (now - rule.LastTriggeredAt.Value).TotalMinutes < rule.CooldownMinutes)
                continue;

            var subject = $"[DocAnalytics] Failure rate {rate:F1}% exceeds {rule.ThresholdPercent}%";
            var body =
                $"Alert rule: {rule.Name}\n" +
                $"Site: {rule.SiteId}\n" +
                $"Window: last {rule.WindowMinutes} min\n" +
                $"Failed {failed} of {total} files ({rate:F1}%).\n" +
                $"Threshold: {rule.ThresholdPercent}%.\n" +
                $"Time (UTC): {now:yyyy-MM-dd HH:mm}Z";

            try
            {
                await _email.SendAsync(rule.Email, subject, body, ct);
                // ── NEW: persist an in-app notification so it surfaces on next login ──
                var severity = rate >= rule.ThresholdPercent * 1.5 ? "critical" : "warning";
                _db.AlertNotifications.Add(new AlertNotification
                {
                    Id = Guid.NewGuid(),
                    TenantId = rule.TenantId,     // rule is ITenantScoped → scope explicitly
                    SiteId = rule.SiteId,
                    AlertRuleId = rule.Id,
                    RuleName = rule.Name,
                    Message = $"Failure rate {rate:F1}% exceeded threshold "
                                     + $"{rule.ThresholdPercent}% over the last {rule.WindowMinutes} min "
                                     + $"({failed} of {total} files).",
                    Severity = severity,
                    ObservedPercent = Math.Round(rate, 2),
                    ThresholdPercent = rule.ThresholdPercent,
                    IsRead = false,
                    FiredAt = now
                });

                rule.LastTriggeredAt = now;
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Alert '{Name}' fired for site {Site} ({Rate:F1}%).",
                    rule.Name, rule.SiteId, rate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send alert '{Name}'.", rule.Name);
            }
        }
    }
}
