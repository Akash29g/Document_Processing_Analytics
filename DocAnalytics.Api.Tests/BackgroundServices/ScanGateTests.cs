using DocAnalytics.Api.BackgroundServices;

namespace DocAnalytics.Api.Tests.BackgroundServices;

public class ScanGateTests
{
    [Theory]
    [InlineData("NO_THREATS_FOUND", ScanDecision.Clean)]
    [InlineData("THREATS_FOUND", ScanDecision.Threat)]
    [InlineData(null, ScanDecision.Incomplete)]  // still pending after timeout
    [InlineData("FAILED", ScanDecision.Incomplete)]
    [InlineData("UNSUPPORTED", ScanDecision.Incomplete)]
    [InlineData("ACCESS_DENIED", ScanDecision.Incomplete)]
    [InlineData("something_weird", ScanDecision.Incomplete)]  // unknown → fail-closed
    public void Evaluate_maps_scan_status_to_decision(string? status, ScanDecision expected)
        => Assert.Equal(expected, ScanGate.Evaluate(status));
}
