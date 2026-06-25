using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Invoices;

public static class InvoiceFeatureExtensions
{
    public static IServiceCollection AddInvoiceFeature(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        return services;
    }
}
