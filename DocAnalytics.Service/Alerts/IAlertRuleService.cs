namespace DocAnalytics.Service.Alerts;

public interface IAlertRuleService
{
    Task<IReadOnlyList<AlertRuleDto>> ListAsync(CancellationToken ct = default);
    Task<AlertRuleDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<AlertRuleDto> CreateAsync(CreateAlertRuleRequest req, CancellationToken ct = default);
    Task<AlertRuleDto?> UpdateAsync(Guid id, UpdateAlertRuleRequest req, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RecipientDto>> ListRecipientsAsync(CancellationToken ct = default);

}
