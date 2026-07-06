namespace DocAnalytics.Service.Alerts;

// serializes to snake_case globally (threshold_percent, window_minutes, ...)
public sealed class AlertRuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double ThresholdPercent { get; set; }
    public int WindowMinutes { get; set; }
    public string Email { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public int CooldownMinutes { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class CreateAlertRuleRequest
{
    public string Name { get; set; } = null!;
    public double ThresholdPercent { get; set; }
    public int WindowMinutes { get; set; } = 60;
    public string Email { get; set; } = null!;
    public int CooldownMinutes { get; set; } = 60;
    public bool IsEnabled { get; set; } = true;
}

public sealed class UpdateAlertRuleRequest
{
    public string Name { get; set; } = null!;
    public double ThresholdPercent { get; set; }
    public int WindowMinutes { get; set; } = 60;
    public string Email { get; set; } = null!;
    public int CooldownMinutes { get; set; } = 60;
    public bool IsEnabled { get; set; }
}
