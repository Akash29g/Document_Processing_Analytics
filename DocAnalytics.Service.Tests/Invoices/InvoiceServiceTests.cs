using DocAnalytics.Domain.Entities;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Service.Tests.Support;
using MockQueryable.Moq;
using Moq;

namespace DocAnalytics.Service.Tests.Invoices;

public class InvoiceServiceTests
{
    [Fact]
    public async Task GetInvoiceForFileAsync_returns_null_when_file_missing()
    {
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(Array.Empty<FileRecord>().BuildMockDbSet().Object);
        ctx.Setup(c => c.InvoiceLineItems).Returns(Array.Empty<InvoiceLineItem>().BuildMockDbSet().Object);

        var result = await new InvoiceService(ctx.Object).GetInvoiceForFileAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetInvoiceForFileAsync_returns_items_and_grand_total()
    {
        var fileId = Guid.NewGuid();
        var files = new[] { new FileRecord { Id = fileId, FileName = "inv.pdf" } };
        var cat = new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "HW", CategoryName = "Hardware" };
        var items = new[]
        {
            new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileId, LineNumber = 1, Description = "Bolt", LineTotal = 10m, ItemCategory = cat },
            new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileId, LineNumber = 2, Description = "Nut",  LineTotal = 5m,  ItemCategory = null },
        };
        var ctx = MockDb.Create();
        ctx.Setup(c => c.Files).Returns(files.BuildMockDbSet().Object);
        ctx.Setup(c => c.InvoiceLineItems).Returns(items.BuildMockDbSet().Object);

        var result = await new InvoiceService(ctx.Object).GetInvoiceForFileAsync(fileId);

        Assert.NotNull(result);
        Assert.Equal(2, result!.LineItemCount);
        Assert.Equal(15m, result.GrandTotal);          // 10 + 5
        Assert.Equal("Hardware", result.Items[0].CategoryName);
        Assert.Null(result.Items[1].CategoryName);      // no category → null
    }
}
