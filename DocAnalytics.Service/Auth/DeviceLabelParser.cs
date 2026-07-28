namespace DocAnalytics.Service.Auth;

/// <summary>Best-effort "Browser on OS" label from a raw User-Agent string. No UA-parsing library needed —
/// this is a settings-page nicety, not a security control.</summary>
public static class DeviceLabelParser
{
    public static string Parse(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent)) return "Unknown device";

        var ua = userAgent;

        string browser =
            ua.Contains("Edg/") ? "Edge" :
            ua.Contains("OPR/") || ua.Contains("Opera") ? "Opera" :
            ua.Contains("Chrome/") && !ua.Contains("Chromium") ? "Chrome" :
            ua.Contains("Firefox/") ? "Firefox" :
            ua.Contains("Safari/") && !ua.Contains("Chrome") ? "Safari" :
            "Unknown browser";

        string os =
            ua.Contains("Windows") ? "Windows" :
            ua.Contains("Mac OS X") ? "macOS" :
            ua.Contains("Android") ? "Android" :
            (ua.Contains("iPhone") || ua.Contains("iPad")) ? "iOS" :
            ua.Contains("Linux") ? "Linux" :
            "Unknown OS";

        return $"{browser} on {os}";
    }
}
