namespace DocAnalytics.Service.Invoices;

public interface IInvoiceService
{
    Task<InvoiceDetailDto?> GetInvoiceForFileAsync(Guid fileId, CancellationToken ct = default);
}
