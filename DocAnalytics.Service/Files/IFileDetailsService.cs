namespace DocAnalytics.Service.Files;

public interface IFileDetailsService
{
    Task<LookupResult<FileDetailDto>> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default);
    Task<LookupResult<FileLogDto>> GetFileLogsAsync(Guid fileId, CancellationToken ct = default);
}
