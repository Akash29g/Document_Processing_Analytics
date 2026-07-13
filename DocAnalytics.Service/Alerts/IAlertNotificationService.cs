namespace DocAnalytics.Service.Alerts;

public interface IAlertNotificationService
{
    Task<List<AlertNotificationDto>> GetNotificationsAsync(bool unreadOnly, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(CancellationToken ct = default);
    Task<bool> MarkReadAsync(Guid id, CancellationToken ct = default);
    Task<int> MarkAllReadAsync(CancellationToken ct = default);
}
