using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Provisioning;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class ProvisioningControllerTests
{
    private static ProvisioningController Make(Mock<IProvisioningService> svc) =>
        new(svc.Object, Mock.Of<ICurrentUser>());

    [Fact]
    public async Task GetTenants_returns_200_with_list()
    {
        var svc = new Mock<IProvisioningService>();
        svc.Setup(s => s.GetTenantsAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<TenantSummaryDto> { new(Guid.NewGuid(), "Acme", "acme.com", true, 2, 3, 1) });

        var ok = Assert.IsType<OkObjectResult>(await Make(svc).GetTenants(default));
        var body = Assert.IsType<ApiResponse<List<TenantSummaryDto>>>(ok.Value);
        Assert.Single(body.Data!);
    }

    [Fact]
    public async Task CreateTenant_returns_409_when_domain_taken()
    {
        var svc = new Mock<IProvisioningService>();
        svc.Setup(s => s.CreateTenantAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((TenantSummaryDto?)null);

        var conflict = Assert.IsType<ConflictObjectResult>(
            await Make(svc).CreateTenant(new CreateTenantRequest("Acme", "acme.com"), default));
        var body = Assert.IsType<ApiResponse<object>>(conflict.Value);
        Assert.Equal("DOMAIN_TAKEN", body.Error!.Code);
    }

    [Fact]
    public async Task CreateAdmin_returns_404_when_tenant_missing()
    {
        var svc = new Mock<IProvisioningService>();
        svc.Setup(s => s.CreateAdminAsync(It.IsAny<Guid>(), It.IsAny<CreateAdminRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((ProvisionedUserDto?)null);

        var nf = Assert.IsType<NotFoundObjectResult>(
            await Make(svc).CreateAdmin(Guid.NewGuid(), new CreateAdminRequest("A", "B"), default));
        var body = Assert.IsType<ApiResponse<object>>(nf.Value);
        Assert.Equal("NOT_FOUND", body.Error!.Code);
    }

    [Fact]
    public async Task RemoveAdmin_returns_200_when_removed()
    {
        var svc = new Mock<IProvisioningService>();
        svc.Setup(s => s.RemoveAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        Assert.IsType<OkObjectResult>(await Make(svc).RemoveAdmin(Guid.NewGuid(), Guid.NewGuid(), default));
    }

    [Fact]
    public async Task RemoveSite_returns_404_when_missing()
    {
        var svc = new Mock<IProvisioningService>();
        svc.Setup(s => s.RemoveSiteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        Assert.IsType<NotFoundObjectResult>(await Make(svc).RemoveSite(Guid.NewGuid(), Guid.NewGuid(), default));
    }
}
