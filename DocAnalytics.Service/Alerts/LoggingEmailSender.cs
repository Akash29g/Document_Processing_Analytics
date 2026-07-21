using Microsoft.Extensions.Logging;

namespace DocAnalytics.Service.Alerts;

/// <summary>Development <see cref="IEmailSender"/> that "sends" by logging — no SMTP/SES required.</summary>
// Dev default: "sends" by logging. No SMTP/SES needed to test the whole feature.
public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;
    public LoggingEmailSender(ILogger<LoggingEmailSender> logger) => _logger = logger;

    /// <inheritdoc />
    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogWarning("📧 [ALERT EMAIL] To: {To} | Subject: {Subject}\n{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
