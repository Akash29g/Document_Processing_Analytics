namespace DocAnalytics.Service.Alerts;

/// <summary>Abstraction for sending outbound email (implemented by logging and SMTP senders).</summary>
public interface IEmailSender
{
    /// <summary>Sends an email message.</summary>
    /// <param name="to">Recipient address (comma-separated for multiple).</param>
    /// <param name="subject">Message subject.</param>
    /// <param name="body">Message body.</param>
    /// <param name="ct">Cancellation token.</param>
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
