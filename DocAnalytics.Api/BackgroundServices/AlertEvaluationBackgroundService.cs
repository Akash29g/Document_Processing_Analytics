using DocAnalytics.Service.Alerts;
using System.Diagnostics.CodeAnalysis;


namespace DocAnalytics.Api.BackgroundServices;

[ExcludeFromCodeCoverage]
public sealed class AlertEvaluationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<AlertEvaluationBackgroundService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    public AlertEvaluationBackgroundService(
        IServiceScopeFactory scopes, ILogger<AlertEvaluationBackgroundService> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                // scoped AppDbContext lives inside this scope (background has none by default)
                using var scope = _scopes.CreateScope();
                var evaluator = scope.ServiceProvider.GetRequiredService<IAlertEvaluator>();
                await evaluator.EvaluateAllAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alert evaluation tick failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
