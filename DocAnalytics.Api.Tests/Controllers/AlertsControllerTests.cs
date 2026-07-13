using DocAnalytics.Api.Common;                 // ApiResponse<T>
using DocAnalytics.Api.Controllers;            // AlertsController
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Alerts;             // IAlertNotificationService, AlertNotificationDto
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class AlertsControllerTests
{
    // Build the controller with mocked deps. Adjust the ctor args below to match
    // YOUR AlertsController constructor (see note under this file).
    private static AlertsController Make(Mock<IAlertNotificationService> notifications)
    {
        var rules = new Mock<IAlertRuleService>();
        var me = new Mock<ICurrentUser>();
        return new AlertsController(rules.Object, me.Object, notifications.Object);
    }


    [Fact]
    public async Task GetNotifications_returns_200_with_list()
    {
        var svc = new Mock<IAlertNotificationService>();
        svc.Setup(s => s.GetNotificationsAsync(true, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<AlertNotificationDto>
           {
               new() { RuleName = "High failure rate", Message = "m", Severity = "warning" }
           });

        var ok = Assert.IsType<OkObjectResult>(
            await Make(svc).GetNotifications(unread: true, default));
        var body = Assert.IsType<ApiResponse<List<AlertNotificationDto>>>(ok.Value);
        Assert.Single(body.Data!);
    }

    [Fact]
    public async Task MarkRead_returns_200_when_found()
    {
        var svc = new Mock<IAlertNotificationService>();
        svc.Setup(s => s.MarkReadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        Assert.IsType<OkObjectResult>(
            await Make(svc).MarkRead(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task MarkRead_returns_404_when_missing()
    {
        var svc = new Mock<IAlertNotificationService>();
        svc.Setup(s => s.MarkReadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        var nf = Assert.IsType<NotFoundObjectResult>(
            await Make(svc).MarkRead(Guid.NewGuid(), default));
        var body = Assert.IsType<ApiResponse<object>>(nf.Value);
        Assert.Equal("NOT_FOUND", body.Error!.Code);
    }

    [Fact]
    public async Task MarkAllRead_returns_200_with_count()
    {
        var svc = new Mock<IAlertNotificationService>();
        svc.Setup(s => s.MarkAllReadAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(3);

        Assert.IsType<OkObjectResult>(await Make(svc).MarkAllRead(default));
    }
}
