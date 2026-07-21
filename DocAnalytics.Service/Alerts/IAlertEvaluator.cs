namespace DocAnalytics.Service.Alerts;

/// <summary>Evaluates all enabled alert rules and fires notifications/emails when thresholds are exceeded.</summary>
public interface IAlertEvaluator
{
    /// <summary>Scans every enabled rule across all sites and triggers any whose failure rate exceeds its threshold.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task EvaluateAllAsync(CancellationToken ct = default);
}
