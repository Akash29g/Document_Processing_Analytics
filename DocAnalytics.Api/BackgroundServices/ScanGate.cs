namespace DocAnalytics.Api.BackgroundServices;

/// <summary>Decision from a GuardDuty Malware Protection for S3 scan verdict.</summary>
public enum ScanDecision
{
    Clean,       // NO_THREATS_FOUND → safe to extract
    Threat,      // THREATS_FOUND → delete + fail
    Incomplete   // null/pending, FAILED, UNSUPPORTED, ACCESS_DENIED → fail-closed
}

public static class ScanGate
{
    /// <summary>Maps the GuardDutyMalwareScanStatus tag value to a gate decision (fail-closed).</summary>
    public static ScanDecision Evaluate(string? status) => status switch
    {
        "NO_THREATS_FOUND" => ScanDecision.Clean,
        "THREATS_FOUND" => ScanDecision.Threat,
        _ => ScanDecision.Incomplete
    };
}
