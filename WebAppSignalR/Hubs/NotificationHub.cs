using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAppSignalR.Hubs;

public class NotificationHub : Hub
{
    //[Authorize]
    public async Task SendMessage(string user, string message)
    {
        // execute any code and/or call Client
        await Task.CompletedTask;
    }
}
