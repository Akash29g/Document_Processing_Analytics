using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Errors;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;

namespace DocAnalytics.Service.Tests.Errors;

public class ErrorServiceTests
{
    private static ErrorService BuildSut(
        IEnumerable<FileRecord> files, IEnumerable<FileStepHistory> steps,
        IEnumerable<Transaction> txns, IEnumerable<ErrorCatalog> catalog)
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.FileStepHistory).Returns(steps.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.Transactions).Returns(txns.ToList().BuildMockDbSet().Object);
        ctx.Setup(c => c.ErrorCatalog).Returns(catalog.ToList().BuildMockDbSet().Object);
        return new ErrorService(ctx.Object);
    }

    [Fact]
    public async Task GetErrorsAsync_returns_only_failed_steps_with_remediation()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "inv.pdf", TransactionId = txnId } };
        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate",
                                  Status = "Failed", ErrorCode = "ERR1", CompletedAt = DateTime.UtcNow },
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Upload", Status = "Success" },
        };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };
        var catalog = new[] { new ErrorCatalog { ErrorCode = "ERR1", RemediationMsg = "Retry the upload" } };

        var sut = BuildSut(files, steps, txns, catalog);

        var result = await sut.GetErrorsAsync(new ErrorListQuery());

        Assert.Equal(1, result.TotalCount);                        // Success step excluded
        Assert.Equal("Retry the upload", result.Items[0].SuggestedFix); // LEFT-join resolved
    }

    [Fact]
    public async Task GetErrorsAsync_filters_by_step()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", TransactionId = txnId } };
        var steps = new[]
        {
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1" },
            new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Transform", Status = "Failed", ErrorCode = "E2" },
        };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };

        var sut = BuildSut(files, steps, txns, Array.Empty<ErrorCatalog>());

        var result = await sut.GetErrorsAsync(new ErrorListQuery { Step = "Transform" });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Transform", result.Items[0].Step);
        Assert.Null(result.Items[0].SuggestedFix); // empty catalog → null
    }
    [Fact]
    public async Task GetErrorsAsync_filters_by_source()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", TransactionId = txnId } };
        var steps = new[] { new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1" } };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };
        var sut = BuildSut(files, steps, txns, Array.Empty<ErrorCatalog>());

        Assert.Equal(1, (await sut.GetErrorsAsync(new ErrorListQuery { Source = "SAP" })).TotalCount);
        Assert.Equal(0, (await sut.GetErrorsAsync(new ErrorListQuery { Source = "CSV" })).TotalCount);
    }

    [Fact]
    public async Task GetErrorsAsync_filters_by_date_range()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", TransactionId = txnId } };
        var steps = new[]
        {
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1", CompletedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Load", Status = "Failed", ErrorCode = "E2", CompletedAt = new DateTime(2026,6,1,0,0,0,DateTimeKind.Utc) },
    };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };
        var sut = BuildSut(files, steps, txns, Array.Empty<ErrorCatalog>());

        var result = await sut.GetErrorsAsync(new ErrorListQuery { From = new DateTime(2026, 5, 1), To = new DateTime(2026, 7, 1) });
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetErrorsAsync_sorts_by_error_code_descending()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", TransactionId = txnId } };
        var steps = new[]
        {
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1" },
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Load", Status = "Failed", ErrorCode = "E9" },
    };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };
        var sut = BuildSut(files, steps, txns, Array.Empty<ErrorCatalog>());

        var result = await sut.GetErrorsAsync(new ErrorListQuery { SortBy = "error_code", SortDir = "desc" });
        Assert.Equal("E9", result.Items[0].ErrorCode);
    }

    [Fact]
    public async Task GetErrorsForExportAsync_returns_all_matching_rows()
    {
        var fileId = Guid.NewGuid(); var txnId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "a.pdf", TransactionId = txnId } };
        var steps = new[]
        {
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Validate", Status = "Failed", ErrorCode = "E1" },
        new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, StepName = "Load", Status = "Failed", ErrorCode = "E2" },
    };
        var txns = new[] { new Transaction { Id = txnId, SourceSystem = "SAP" } };
        var sut = BuildSut(files, steps, txns, Array.Empty<ErrorCatalog>());

        Assert.Equal(2, (await sut.GetErrorsForExportAsync(new ErrorListQuery())).Count);
    }

}
