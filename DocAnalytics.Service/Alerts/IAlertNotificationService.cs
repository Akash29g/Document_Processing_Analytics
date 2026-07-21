namespace DocAnalytics.Service.Alerts;

/// <summary>Reads and updates in-app alert notifications for the current tenant/site.</summary>
public interface IAlertNotificationService
{
    /// <summary>Returns recent alert notifications (most recent first, capped).</summary>
    /// <param name="unreadOnly">When <c>true</c>, returns only unread notifications.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The notifications.</returns>
    Task<List<AlertNotificationDto>> GetNotificationsAsync(bool unreadOnly, CancellationToken ct = default);

    /// <summary>Returns the count of unread notifications.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The unread count.</returns>
    Task<int> GetUnreadCountAsync(CancellationToken ct = default);

    /// <summary>Marks a single notification as read.</summary>
    /// <param name="id">The notification id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if marked; <c>false</c> if not found.</returns>
    Task<bool> MarkReadAsync(Guid id, CancellationToken ct = default);

    /// <summary>Marks all notifications for the current tenant/site as read.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of notifications marked read.</returns>
    Task<int> MarkAllReadAsync(CancellationToken ct = default);
}
