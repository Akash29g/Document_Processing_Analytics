using System.Diagnostics.CodeAnalysis;
using DocAnalytics.Service.Alerts;


namespace DocAnalytics.Api.BackgroundServices;

/// <summary>Hosted service that periodically evaluates all alert rules (once per minute) via a scoped <see cref="IAlertEvaluator"/>.</summary>
[ExcludeFromCodeCoverage]
public sealed class AlertEvaluationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<AlertEvaluationBackgroundService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    /// <summary>Creates the background service with a scope factory and logger.</summary>
    /// <param name="scopes">Factory used to create a DI scope per tick.</param>
    /// <param name="logger">The logger.</param>
    public AlertEvaluationBackgroundService(
        IServiceScopeFactory scopes, ILogger<AlertEvaluationBackgroundService> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }

    /// <inheritdoc />
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
