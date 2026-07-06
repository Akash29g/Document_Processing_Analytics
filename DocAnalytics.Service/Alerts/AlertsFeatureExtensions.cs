using DocAnalytics.Service.Alerts;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class AlertsFeatureExtensions
{
    public static IServiceCollection AddAlertsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAlertRuleService, AlertRuleService>();
        services.AddScoped<IAlertEvaluator, AlertEvaluator>();

        // Dev default = log emails. For real email, comment this and use the SMTP line below.
        services.AddSingleton<IEmailSender, LoggingEmailSender>();
        // services.AddSingleton<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
