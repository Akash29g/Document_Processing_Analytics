using DocAnalytics.Data;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Realtime;

// Dev-only: simulates a pipeline event so real-time updates are demoable on a
// system that is otherwise read-only after seeding. Runs on an HTTP request, so
// TenantSiteMiddleware has populated CurrentUser → all queries are tenant/site scoped.
public sealed class SimulationService : ISimulationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;
    private readonly IPipelineNotifier _notifier;

    public SimulationService(AppDbContext db, ICurrentUser me, IPipelineNotifier notifier)
    {
        _db = db; _me = me; _notifier = notifier;
    }

    private static readonly string[] Outcomes = { "Completed", "Failed" };

    public async Task<FileStateChangedNotification?> SimulateStateChangeAsync(CancellationToken ct = default)
    {
        // Prefer an in-flight file; fall back to any file for this site.
        var query = _db.Files.Where(f => f.Status == "Processing" || f.Status == "Queued");
        var count = await query.CountAsync(ct);
        if (count == 0)
        {
            query = _db.Files;
            count = await query.CountAsync(ct);
        }
        if (count == 0) return null;

        var skip = Random.Shared.Next(count);
        var file = await query.OrderBy(f => f.Id).Skip(skip).Take(1).FirstOrDefaultAsync(ct);
        if (file is null) return null;

        var oldState = file.Status;
        var newState = Outcomes[Random.Shared.Next(Outcomes.Length)];
        var now = DateTime.UtcNow;

        file.Status = newState;
        file.CurrentStep = newState == "Completed" ? "Publish" : "Validate";
        file.LastUpdatedAt = now;

        // keep the parent transaction counters roughly consistent
        var txn = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == file.TransactionId, ct);
        if (txn is not null)
        {
            Decrement(txn, oldState);
            Increment(txn, newState);
            txn.LastUpdatedAt = now;
        }

        // audit row (ActivityLog is ITenantScoped → set tenant/site explicitly on insert)
        _db.ActivityLog.Add(new DocAnalytics.Domain.Entities.ActivityLog
        {
            Id = Guid.NewGuid(),
            TenantId = _me.TenantId,
            SiteId = _me.SiteId,
            EventType = "FILE_STATE_CHANGED",
            EntityType = "File",
            EntityId = file.Id,
            EntityName = file.FileName,
            OldState = oldState,
            NewState = newState,
            TriggeredBy = "simulator",
            CreatedAt = now,
        });

        await _db.SaveChangesAsync(ct);

        var notification = new FileStateChangedNotification(
            file.Id, file.FileName, oldState, newState, file.CurrentStep, now);

        // push to this site's group
        await _notifier.NotifyFileStateChangedAsync(file.SiteId, notification, ct);

        return notification;
    }

    private static void Increment(Transaction t, string state)
    {
        switch (state)
        {
            case "Queued": t.UploadedCount++; break;
            case "Processing": t.ProcessingCount++; break;
            case "Completed": t.CompletedCount++; break;
            case "Failed": t.FailedCount++; break;
        }
    }

    private static void Decrement(Transaction t, string state)
    {
        switch (state)
        {
            case "Queued": if (t.UploadedCount > 0) t.UploadedCount--; break;
            case "Processing": if (t.ProcessingCount > 0) t.ProcessingCount--; break;
            case "Completed": if (t.CompletedCount > 0) t.CompletedCount--; break;
            case "Failed": if (t.FailedCount > 0) t.FailedCount--; break;
        }
    }
}
