using System.Text;
using DocAnalytics.Data;                 // AppDbContext
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Files;

public sealed class FileDetailsService : IFileDetailsService
{
    private readonly AppDbContext _db;
    public FileDetailsService(AppDbContext db) => _db = db;

    // GET /api/v1/files/{id}/details — joins Files + FileStepHistory + ErrorCatalog
    public async Task<FileDetailDto?> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default)
    {
        // 1) Load the file SCOPED to this tenant/site (global query filter auto-applies).
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;   // 404 for both not-found AND other-tenant (no existence leak)

        // 2) Pull this file's steps in timeline order. (FileStepHistory is NOT tenant-scoped,
        //    so we always drive from the already-scoped file id — isolation stays intact.)
        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        // 3) Soft-join to ErrorCatalog BY error_code (one round-trip, no N+1).
        var codes = steps.Where(s => s.ErrorCode != null)
                         .Select(s => s.ErrorCode!)
                         .Distinct()
                         .ToList();

        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        // 4) Shape the nested DTO.
        var dto = new FileDetailDto
        {
            FileInfo = new FileInfoDto
            {
                Id = file.Id,
                Name = file.FileName,
                CurrentStatus = file.Status,
                CurrentStep = file.CurrentStep
            },
            History = steps.Select(s => new StepHistoryDto
            {
                Step = s.StepName,
                Status = s.Status,
                Ts = s.StartedAt ?? s.CompletedAt,
                Error = s.ErrorCode is null ? null : new StepErrorDto
                {
                    Code = s.ErrorCode,
                    Message = s.ErrorMessage,
                    SuggestedFix = remediation.TryGetValue(s.ErrorCode, out var fix) ? fix : null
                }
            }).ToList()
        };

        return dto;
    }

    // GET /api/v1/files/{id}/logs — downloadable step-by-step trace
    public async Task<FileLogDto?> GetFileLogsAsync(Guid fileId, CancellationToken ct = default)
    {
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;

        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        var codes = steps.Where(s => s.ErrorCode != null).Select(s => s.ErrorCode!).Distinct().ToList();
        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        var sb = new StringBuilder();
        sb.AppendLine("=== Document Processing — File Step Log ===");
        sb.AppendLine($"File          : {file.FileName}");
        sb.AppendLine($"File Id       : {file.Id}");
        sb.AppendLine($"Current Status: {file.Status}");
        sb.AppendLine($"Current Step  : {file.CurrentStep}");
        sb.AppendLine($"Generated     : {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}");
        sb.AppendLine(new string('-', 60));

        foreach (var s in steps)
        {
            var ts = (s.StartedAt ?? s.CompletedAt)?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "(no timestamp)";
            sb.AppendLine($"[{ts}] {s.StepName,-12} {s.Status}");
            if (s.ErrorCode is not null)
            {
                sb.AppendLine($"    Error      : {s.ErrorCode} — {s.ErrorMessage}");
                if (remediation.TryGetValue(s.ErrorCode, out var fix) && !string.IsNullOrWhiteSpace(fix))
                    sb.AppendLine($"    Suggested  : {fix}");
            }
        }

        return new FileLogDto
        {
            FileName = $"file_{file.Id}_log.txt",
            Content = sb.ToString()
        };
    }



}
