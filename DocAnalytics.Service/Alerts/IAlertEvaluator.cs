namespace DocAnalytics.Service.Alerts;

public interface IAlertEvaluator
{
    Task EvaluateAllAsync(CancellationToken ct = default);
}
