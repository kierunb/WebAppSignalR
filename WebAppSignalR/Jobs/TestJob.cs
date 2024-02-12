using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace WebAppSignalR.Jobs;

public class TestJob(
    ILogger<TestJob> logger, 
    IHubContext<ChatHub> hubContext) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation("TestJob invoked.");

        await hubContext.Clients.All.SendAsync("ReceiveMessage", "Scheduler", ">>> TestJob invoked.");
    }
}
