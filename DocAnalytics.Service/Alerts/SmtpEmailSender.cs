using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace DocAnalytics.Service.Alerts;

[ExcludeFromCodeCoverage]
// Reads the "Email" section of appsettings. Uses built-in SmtpClient (no extra package).
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _cfg;
    public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var s = _cfg.GetSection("Email");
        var from = s["From"] ?? "alerts@docanalytics.local";

        using var msg = new MailMessage { From = new MailAddress(from), Subject = subject, Body = body };
        foreach (var addr in to.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            msg.To.Add(addr);

        using var client = new SmtpClient(s["Host"], int.TryParse(s["Port"], out var p) ? p : 587)
        {
            EnableSsl = bool.TryParse(s["UseSsl"], out var ssl) && ssl,
            Credentials = new NetworkCredential(s["User"], s["Password"])
        };
        await client.SendMailAsync(msg, ct);
    }
}
