using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.AdminUsers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class AdminControllerTests
{
    [Fact]
    public async Task GetUsers_returns_200_with_list()
    {
        var svc = new Mock<IAdminUserService>();
        svc.Setup(s => s.GetUsersAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<AdminUserDto>
               { new(Guid.NewGuid(), "u@acme.com", "Viewer", true, DateTime.UtcNow, new List<Guid>()) });

        var ok = Assert.IsType<OkObjectResult>(await new AdminController(svc.Object).GetUsers(default));
        var body = Assert.IsType<ApiResponse<List<AdminUserDto>>>(ok.Value);
        Assert.Single(body.Data!);
    }

    [Fact]
    public async Task CreateUser_returns_400_on_invalid_sites()
    {
        var svc = new Mock<IAdminUserService>();
        svc.Setup(s => s.CreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((AdminCreatedUserDto?)null);

        var bad = Assert.IsType<BadRequestObjectResult>(await new AdminController(svc.Object)
            .CreateUser(new AdminCreateUserRequest("A", "B", new List<Guid> { Guid.NewGuid() }), default));
        var body = Assert.IsType<ApiResponse<object>>(bad.Value);
        Assert.Equal("INVALID_SITES", body.Error!.Code);
    }

    [Fact]
    public async Task CreateUser_returns_200_with_created_email()
    {
        var svc = new Mock<IAdminUserService>();
        svc.Setup(s => s.CreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new AdminCreatedUserDto(Guid.NewGuid(), "a.b@acme.com", true));

        var ok = Assert.IsType<OkObjectResult>(await new AdminController(svc.Object)
            .CreateUser(new AdminCreateUserRequest("A", "B", new List<Guid> { Guid.NewGuid() }), default));
        var body = Assert.IsType<ApiResponse<AdminCreatedUserDto>>(ok.Value);
        Assert.Equal("a.b@acme.com", body.Data!.Email);
    }

    [Fact]
    public async Task DeactivateUser_returns_404_when_not_in_tenant()
    {
        var svc = new Mock<IAdminUserService>();
        svc.Setup(s => s.DeactivateUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        Assert.IsType<NotFoundObjectResult>(
            await new AdminController(svc.Object).DeactivateUser(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task UpdateUserSites_returns_200_when_updated()
    {
        var svc = new Mock<IAdminUserService>();
        svc.Setup(s => s.UpdateUserSitesAsync(It.IsAny<Guid>(), It.IsAny<UpdateUserSitesRequest>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        Assert.IsType<OkObjectResult>(await new AdminController(svc.Object)
            .UpdateUserSites(Guid.NewGuid(), new UpdateUserSitesRequest(new List<Guid> { Guid.NewGuid() }), default));
    }
}
