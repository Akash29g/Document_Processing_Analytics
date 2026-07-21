namespace DocAnalytics.Service.Invoices;

/// <summary>Reads the extracted invoice (header + line items) for a file.</summary>
public interface IInvoiceService
{
    /// <summary>Returns the invoice detail extracted from a file.</summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The invoice detail, or <c>null</c> if the file isn't found for the current tenant/site.</returns>
    Task<InvoiceDetailDto?> GetInvoiceForFileAsync(Guid fileId, CancellationToken ct = default);
}
