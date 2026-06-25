namespace DocAnalytics.Service.Files;

public interface IFileDetailsService
{
    Task<FileDetailDto?> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default);
    Task<FileLogDto?> GetFileLogsAsync(Guid fileId, CancellationToken ct = default);
}
