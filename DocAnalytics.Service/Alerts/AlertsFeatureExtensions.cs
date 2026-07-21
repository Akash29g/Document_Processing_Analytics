using DocAnalytics.Service.Alerts;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

/// <summary>Dependency-injection registration for the Alerts feature (rules, evaluator, notifications, email).</summary>
public static class AlertsFeatureExtensions
{
    /// <summary>Registers alert services and the default (logging) email sender.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddAlertsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAlertRuleService, AlertRuleService>();
        services.AddScoped<IAlertEvaluator, AlertEvaluator>();
        services.AddScoped<IAlertNotificationService, AlertNotificationService>();

        // Dev default = log emails. For real email, comment this and use the SMTP line below.
        services.AddSingleton<IEmailSender, LoggingEmailSender>();
        // services.AddSingleton<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
