using System.Text;

namespace DocAnalytics.Service.Errors;

public static class ErrorCsvWriter
{
    private static readonly string[] Header =
        { "file_id", "file_name", "error_code", "error_message",
          "step", "source", "failed_at", "suggested_fix" };

    public static string Write(IEnumerable<ErrorListItemDto> rows)
    {
        var sb = new StringBuilder();
        sb.Append(string.Join(',', Header)).Append('\n');

        foreach (var r in rows)
        {
            sb.Append(Escape(r.FileId.ToString())).Append(',')
              .Append(Escape(r.FileName)).Append(',')
              .Append(Escape(r.ErrorCode)).Append(',')
              .Append(Escape(r.ErrorMessage)).Append(',')
              .Append(Escape(r.Step)).Append(',')
              .Append(Escape(r.Source)).Append(',')
              .Append(Escape(r.FailedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"))).Append(',')
              .Append(Escape(r.SuggestedFix))
              .Append('\n');
        }
        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        var needsQuote = value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0;
        var escaped = value.Replace("\"", "\"\"");
        return needsQuote ? $"\"{escaped}\"" : escaped;
    }
}
