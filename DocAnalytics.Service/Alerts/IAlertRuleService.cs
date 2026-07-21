namespace DocAnalytics.Service.Alerts;

/// <summary>Manages failure-rate alert rules and their eligible recipients (S-4).</summary>
public interface IAlertRuleService
{
    /// <summary>Lists alert rules visible to the current user (Admins see all; Viewers see rules they're a recipient of).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The visible alert rules.</returns>
    Task<IReadOnlyList<AlertRuleDto>> ListAsync(CancellationToken ct = default);

    /// <summary>Gets a single alert rule by id.</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The rule, or <c>null</c> if not found or not visible to the caller.</returns>
    Task<AlertRuleDto?> GetAsync(Guid id, CancellationToken ct = default);

    /// <summary>Creates a new alert rule stamped with the current tenant/site.</summary>
    /// <param name="req">The rule definition.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created rule.</returns>
    Task<AlertRuleDto> CreateAsync(CreateAlertRuleRequest req, CancellationToken ct = default);

    /// <summary>Updates an existing alert rule.</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="req">The updated rule fields.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated rule, or <c>null</c> if not found.</returns>
    Task<AlertRuleDto?> UpdateAsync(Guid id, UpdateAlertRuleRequest req, CancellationToken ct = default);

    /// <summary>Deletes an alert rule.</summary>
    /// <param name="id">The alert rule id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if deleted; <c>false</c> if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

    /// <summary>Lists candidate recipients (active users) for the current site.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The eligible recipients.</returns>
    Task<IReadOnlyList<RecipientDto>> ListRecipientsAsync(CancellationToken ct = default);

}
