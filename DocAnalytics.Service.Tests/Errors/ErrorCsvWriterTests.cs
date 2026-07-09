using DocAnalytics.Service.Errors;

namespace DocAnalytics.Service.Tests.Errors;

public class ErrorCsvWriterTests
{
    [Fact]
    public void Write_emits_header_row()
    {
        var csv = ErrorCsvWriter.Write(Array.Empty<ErrorListItemDto>());
        var header = csv.Split('\n')[0];
        Assert.Equal("file_id,file_name,error_code,error_message,step,source,failed_at,suggested_fix", header);
    }

    [Fact]
    public void Write_serialises_a_row()
    {
        var fileId = Guid.NewGuid();
        var rows = new[]
        {
            new ErrorListItemDto
            {
                FileId = fileId, FileName = "inv.pdf", ErrorCode = "E1", ErrorMessage = "bad",
                Step = "Validate", Source = "SAP",
                FailedAt = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc), SuggestedFix = "Retry"
            }
        };
        var line = ErrorCsvWriter.Write(rows).Split('\n')[1];
        Assert.Contains(fileId.ToString(), line);
        Assert.Contains("inv.pdf", line);
        Assert.Contains("2026-05-01T08:00:00Z", line);
        Assert.Contains("Retry", line);
    }

    [Fact]
    public void Write_quotes_and_escapes_commas_and_quotes()
    {
        var rows = new[]
        {
            new ErrorListItemDto
            {
                FileId = Guid.NewGuid(), FileName = "a,b.pdf", ErrorCode = "E1",
                ErrorMessage = "he said \"hi\"", Step = "Validate", Source = "SAP"
            }
        };
        var line = ErrorCsvWriter.Write(rows).Split('\n')[1];
        Assert.Contains("\"a,b.pdf\"", line);              // comma → wrapped in quotes
        Assert.Contains("\"he said \"\"hi\"\"\"", line);   // inner quotes doubled + wrapped
    }

    [Fact]
    public void Write_emits_empty_fields_for_nulls()
    {
        var rows = new[]
        {
            new ErrorListItemDto
            {
                FileId = Guid.NewGuid(), FileName = "a.pdf", ErrorCode = "E1",
                ErrorMessage = null, Step = "Validate", Source = "SAP", FailedAt = null, SuggestedFix = null
            }
        };
        var line = ErrorCsvWriter.Write(rows).Split('\n')[1];
        Assert.Contains(",,", line);   // consecutive empty fields for the nulls
    }
}
