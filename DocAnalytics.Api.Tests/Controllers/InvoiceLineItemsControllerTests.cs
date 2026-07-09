using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.Invoices;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class InvoiceLineItemsControllerTests
{
    [Fact]
    public async Task GetLineItems_returns_404_when_missing()
    {
        var svc = new Mock<IInvoiceService>();
        svc.Setup(s => s.GetInvoiceForFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((InvoiceDetailDto?)null);

        var nf = Assert.IsType<NotFoundObjectResult>(await new InvoiceLineItemsController(svc.Object).GetLineItems(Guid.NewGuid(), default));
        var body = Assert.IsType<ApiResponse<InvoiceDetailDto>>(nf.Value);
        Assert.Equal("not_found", body.Error!.Code);
    }

    [Fact]
    public async Task GetLineItems_returns_200_when_found()
    {
        var dto = new InvoiceDetailDto { FileId = Guid.NewGuid(), LineItemCount = 2, GrandTotal = 15m };
        var svc = new Mock<IInvoiceService>();
        svc.Setup(s => s.GetInvoiceForFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var ok = Assert.IsType<OkObjectResult>(await new InvoiceLineItemsController(svc.Object).GetLineItems(Guid.NewGuid(), default));
        var body = Assert.IsType<ApiResponse<InvoiceDetailDto>>(ok.Value);
        Assert.Equal(15m, body.Data!.GrandTotal);
    }
}
