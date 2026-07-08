using System.Reflection;
using DocAnalytics.Service.Batches;

namespace DocAnalytics.Tests.Batches;

public class MapStatusToStateTests
{
    private static string? Map(string status)
    {
        var m = typeof(BatchService).GetMethod(
            "MapStatusToState", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string?)m.Invoke(null, new object[] { status });
    }

    [Theory]
    [InlineData("failed", "Failed")]
    [InlineData("completed", "Completed")]
    [InlineData("in_progress", "Processing")]
    [InlineData("queued", "Queued")]          // ← your R4 addition
    [InlineData("FAILED", "Failed")]          // case-insensitive
    [InlineData("banana", null)]              // unknown → null
    [InlineData("", null)]                    // empty → null
    public void MapStatusToState_maps_correctly(string input, string? expected)
        => Assert.Equal(expected, Map(input));
}
