using DocAnalytics.Service.Errors;

namespace DocAnalytics.Tests.Errors;

public class ErrorCsvWriterTests
{
    [Fact]
    public void Write_StartsWith_HeaderRow()
    {
        var csv = ErrorCsvWriter.Write(new List<ErrorListItemDto>());
        var firstLine = csv.Split('\n')[0];

        Assert.Equal(
            "file_id,file_name,error_code,error_message,step,source,failed_at,suggested_fix",
            firstLine);
    }

    [Fact]
    public void Write_EmptyInput_ReturnsHeaderOnly()
    {
        var csv = ErrorCsvWriter.Write(new List<ErrorListItemDto>());

        Assert.Equal(
            "file_id,file_name,error_code,error_message,step,source,failed_at,suggested_fix\n",
            csv);
    }

    [Fact]
    public void Write_QuotesValues_ContainingComma()
    {
        var rows = new List<ErrorListItemDto>
        {
            new() { FileName = "Invoice, final.pdf", ErrorCode = "E1", Step = "Validate", Source = "SAP" }
        };

        var csv = ErrorCsvWriter.Write(rows);

        Assert.Contains("\"Invoice, final.pdf\"", csv);
    }

    [Fact]
    public void Write_EscapesDoubleQuotes_ByDoubling()
    {
        var rows = new List<ErrorListItemDto>
        {
            new() { FileName = "a\"b", ErrorCode = "E1", Step = "Validate", Source = "SAP" }
        };

        var csv = ErrorCsvWriter.Write(rows);

        // a"b  →  "a""b"  (RFC-4180 quote escaping)
        Assert.Contains("\"a\"\"b\"", csv);
    }

    [Fact]
    public void Write_NullErrorMessage_ProducesEmptyField()
    {
        var id = Guid.NewGuid();
        var rows = new List<ErrorListItemDto>
        {
            new() { FileId = id, FileName = "f.pdf", ErrorCode = "E1",
                    ErrorMessage = null, Step = "Load", Source = "SAP" }
        };

        var csv = ErrorCsvWriter.Write(rows);
        var dataLine = csv.Split('\n')[1];

        Assert.Equal($"{id},f.pdf,E1,,Load,SAP,,", dataLine);
    }

    [Fact]
    public void Write_FormatsFailedAt_AsIso8601()
    {
        var when = new DateTime(2026, 7, 8, 9, 30, 0, DateTimeKind.Utc);
        var rows = new List<ErrorListItemDto>
        {
            new() { FileName = "f.pdf", ErrorCode = "E1", Step = "Load", Source = "SAP", FailedAt = when }
        };

        var csv = ErrorCsvWriter.Write(rows);

        Assert.Contains("2026-07-08T09:30:00Z", csv);
    }
}
