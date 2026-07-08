using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Uploads;

public sealed class UploadUrlRequest
{
    [Required] public string FileName { get; set; } = null!;
    [Required] public long SizeBytes { get; set; }
}

public sealed class UploadUrlResponse
{
    public Guid FileId { get; set; }
    public string UploadUrl { get; set; } = null!;
}
