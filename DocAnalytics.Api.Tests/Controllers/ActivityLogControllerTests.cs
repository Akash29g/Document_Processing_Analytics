using DocAnalytics.Api.Common;
using DocAnalytics.Api.Controllers;
using DocAnalytics.Service.ActivityLog;
using DocAnalytics.Service.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocAnalytics.Api.Tests.Controllers;

public class ActivityLogControllerTests
{
    [Fact]
    public async Task GetActivityLog_returns_200_with_list_and_meta()
    {
        var paged = new PagedResult<ActivityLogItemDto>
        {
            Items = new() { new ActivityLogItemDto { EventType = "BATCH_SUBMITTED", EntityType = "Batch", Actor = "system" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        var svc = new Mock<IActivityLogService>();
        svc.Setup(s => s.GetActivityLogAsync(It.IsAny<ActivityLogQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await new ActivityLogController(svc.Object).GetActivityLog(new ActivityLogQuery(), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ApiResponse<List<ActivityLogItemDto>>>(ok.Value);
        Assert.Single(body.Data!);
        Assert.Equal(1, body.Meta!.TotalCount);
    }
}
