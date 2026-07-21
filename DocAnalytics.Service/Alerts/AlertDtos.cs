namespace DocAnalytics.Service.Alerts;

/// <summary>An alert rule as returned to the client (serialized to snake_case).</summary>
// serializes to snake_case globally (threshold_percent, window_minutes, ...)
public sealed class AlertRuleDto
{
    /// <summary>Rule id.</summary>
    public Guid Id { get; set; }
    /// <summary>Human-readable rule name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Failure-rate percentage that triggers the alert.</summary>
    public double ThresholdPercent { get; set; }
    /// <summary>Look-back window in minutes.</summary>
    public int WindowMinutes { get; set; }
    /// <summary>Recipient email(s), comma-separated.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Whether the rule is currently enabled.</summary>
    public bool IsEnabled { get; set; }
    /// <summary>Minimum gap between alerts, in minutes.</summary>
    public int CooldownMinutes { get; set; }
    /// <summary>When the rule last fired (UTC), or null if never.</summary>
    public DateTime? LastTriggeredAt { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Last-updated timestamp (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>Request to create a new alert rule.</summary>
public sealed class CreateAlertRuleRequest
{
    /// <summary>Human-readable rule name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Failure-rate percentage that triggers the alert.</summary>
    public double ThresholdPercent { get; set; }
    /// <summary>Look-back window in minutes.</summary>
    public int WindowMinutes { get; set; } = 60;
    /// <summary>Recipient email(s), comma-separated.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Minimum gap between alerts, in minutes.</summary>
    public int CooldownMinutes { get; set; } = 60;
    /// <summary>Whether the rule is enabled on creation.</summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>A candidate alert recipient (an active site user).</summary>
public sealed class RecipientDto
{
    /// <summary>User id.</summary>
    public Guid Id { get; set; }
    /// <summary>User email.</summary>
    public string Email { get; set; } = null!;
    /// <summary>User role.</summary>
    public string Role { get; set; } = null!;
}


/// <summary>Request to update an existing alert rule.</summary>
public sealed class UpdateAlertRuleRequest
{
    /// <summary>Human-readable rule name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Failure-rate percentage that triggers the alert.</summary>
    public double ThresholdPercent { get; set; }
    /// <summary>Look-back window in minutes.</summary>
    public int WindowMinutes { get; set; } = 60;
    /// <summary>Recipient email(s), comma-separated.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Minimum gap between alerts, in minutes.</summary>
    public int CooldownMinutes { get; set; } = 60;
    /// <summary>Whether the rule is enabled.</summary>
    public bool IsEnabled { get; set; }
}

/// <summary>A fired alert surfaced as an in-app notification.</summary>
public sealed class AlertNotificationDto
{
    /// <summary>Notification id.</summary>
    public Guid Id { get; set; }
    /// <summary>Id of the rule that fired.</summary>
    public Guid AlertRuleId { get; set; }
    /// <summary>Denormalized rule name (no join needed to display).</summary>
    public string RuleName { get; set; } = null!;
    /// <summary>Human-readable notification message.</summary>
    public string Message { get; set; } = null!;
    /// <summary>Severity: info | warning | critical.</summary>
    public string Severity { get; set; } = null!;   // info | warning | critical
    /// <summary>Failure percentage observed when the rule tripped.</summary>
    public double ObservedPercent { get; set; }
    /// <summary>The rule's threshold at fire time.</summary>
    public double ThresholdPercent { get; set; }
    /// <summary>Whether the notification has been read.</summary>
    public bool IsRead { get; set; }
    /// <summary>When the alert fired (UTC).</summary>
    public DateTime FiredAt { get; set; }
    /// <summary>When it was read (UTC), or null if unread.</summary>
    public DateTime? ReadAt { get; set; }
}
