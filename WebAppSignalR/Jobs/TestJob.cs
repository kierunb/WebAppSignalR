using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using WebAppSignalR.Hubs;

namespace WebAppSignalR.Jobs;

public class TestJob(
    ILogger<TestJob> logger, 
    IHubContext<NotificationHub> hubContext) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation("TestJob invoked.");
        await Task.CompletedTask;
    }
}
