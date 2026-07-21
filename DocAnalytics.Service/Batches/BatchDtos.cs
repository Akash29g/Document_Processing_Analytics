using System.ComponentModel.DataAnnotations;
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;


/// <summary>Query-string filters/paging/sort for the batch list (FR-2.1–2.3).</summary>
// The filters/options the client sends in the URL (?page=1&status=failed...)
public sealed class BatchListQuery : IValidatableObject
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;
    /// <summary>Page size (capped at 100).</summary>
    public int PageSize { get; set; } = 20;
    /// <summary>Status filter: all | in_progress | completed | failed.</summary>
    public string? Status { get; set; }   // all | in_progress | completed | failed
    /// <summary>Source-system filter.</summary>
    public string? Source { get; set; }   // filter by source system
    /// <summary>Include batches submitted on/after this instant.</summary>
    public DateTime? From { get; set; }    // submitted on/after
    /// <summary>Include batches submitted on/before this instant.</summary>
    public DateTime? To { get; set; }      // submitted on/before
    /// <summary>Partial batch-id search.</summary>
    public string? Search { get; set; }    // partial batch id
    /// <summary>Column to sort by.</summary>
    public string? SortBy { get; set; }    // which column to sort by

    /// <summary>Sort direction: asc or desc.</summary>
    [OneOf("asc", "desc")]
    public string? SortDir { get; set; }   // asc or desc

    /// <inheritdoc />
    // Cross-field rule: a date window only makes sense if from is on/before to.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Only check when BOTH are supplied — null means "no bound on this side".
        if (From.HasValue && To.HasValue && From > To)
        {
            yield return new ValidationResult(
                "'from' must be earlier than or equal to 'to'.",
                new[] { nameof(From), nameof(To) });
        }
    }
}


/// <summary>One row of the batch list response.</summary>
// One row of the batch list that we send back
public sealed class BatchListItemDto
{
    /// <summary>The batch (transaction) id.</summary>
    public Guid TransactionId { get; set; }
    /// <summary>The batch state.</summary>
    public string State { get; set; } = default!;
    /// <summary>The source system that submitted the batch.</summary>
    public string SourceSystem { get; set; } = default!;
    /// <summary>Total files in the batch.</summary>
    public int TotalFiles { get; set; }
    /// <summary>Count of uploaded files.</summary>
    public int UploadedCount { get; set; }
    /// <summary>Count of files currently processing.</summary>
    public int ProcessingCount { get; set; }
    /// <summary>Count of failed files.</summary>
    public int FailedCount { get; set; }
    /// <summary>Count of completed files.</summary>
    public int CompletedCount { get; set; }
    /// <summary>Submission timestamp (UTC).</summary>
    public DateTime SubmittedAt { get; set; }
    /// <summary>Last-updated timestamp (UTC).</summary>
    public DateTime LastUpdatedAt { get; set; }
    /// <summary>Completion timestamp (UTC), or null if not complete.</summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Batch drill-down detail (FR-2.4).</summary>
// ── GET /api/v1/batches/{id} : drill-down detail ──
public sealed class BatchDetailDto
{
    /// <summary>The batch id.</summary>
    public Guid Id { get; set; }
    /// <summary>The batch state (from Transaction.State).</summary>
    public string Status { get; set; } = null!;   // from Transaction.State
    /// <summary>The source system (from Transaction.SourceSystem).</summary>
    public string Source { get; set; } = null!;    // from Transaction.SourceSystem
    /// <summary>Total files in the batch.</summary>
    public int TotalFiles { get; set; }
    /// <summary>Per-status file counts.</summary>
    public FileStatsDto FileStats { get; set; } = null!;   // → "file_stats"
    /// <summary>Batch timing information.</summary>
    public BatchTimesDto Times { get; set; } = null!;      // → "times"
}

/// <summary>Per-status file counts within a batch.</summary>
public sealed class FileStatsDto
{
    /// <summary>Uploaded file count.</summary>
    public int Uploaded { get; set; }
    /// <summary>Processing file count.</summary>
    public int Processing { get; set; }
    /// <summary>Failed file count.</summary>
    public int Failed { get; set; }
    /// <summary>Completed file count.</summary>
    public int Completed { get; set; }
}

/// <summary>Batch timing information.</summary>
public sealed class BatchTimesDto
{
    /// <summary>Submission timestamp (UTC).</summary>
    public DateTime SubmittedAt { get; set; }
    /// <summary>Last-updated timestamp (UTC).</summary>
    public DateTime LastUpdatedAt { get; set; }
    /// <summary>Completion timestamp (UTC), or null.</summary>
    public DateTime? CompletedAt { get; set; }   // nullable
}

/// <summary>One file row within a batch (FR-2.4).</summary>
// ── GET /api/v1/batches/{id}/files : one file row ──
public sealed class BatchFileDto
{
    /// <summary>The file id.</summary>
    public Guid Id { get; set; }
    /// <summary>The file name.</summary>
    public string FileName { get; set; } = null!;
    /// <summary>The file type/extension.</summary>
    public string FileType { get; set; } = null!;
    /// <summary>The current file status.</summary>
    public string Status { get; set; } = null!;
    /// <summary>The current pipeline step.</summary>
    public string CurrentStep { get; set; } = null!;
    /// <summary>File size in bytes, if known.</summary>
    public long? FileSizeBytes { get; set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Last-updated timestamp (UTC).</summary>
    public DateTime LastUpdatedAt { get; set; }
}

/// <summary>Pagination parameters for listing files within a batch.</summary>
public sealed class BatchFilesQuery
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;
    /// <summary>Page size (capped at 100 by the service).</summary>
    public int PageSize { get; set; } = 20;
}
