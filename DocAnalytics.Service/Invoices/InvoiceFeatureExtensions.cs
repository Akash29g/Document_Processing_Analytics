using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Invoices;

/// <summary>Dependency-injection registration for the Invoice feature.</summary>
public static class InvoiceFeatureExtensions
{
    /// <summary>Registers the invoice service in the DI container.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddInvoiceFeature(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        return services;
    }
}
