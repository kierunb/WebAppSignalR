using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAppSignalR.Hubs;

namespace WebAppSignalR.Services;

// docs: https://learn.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-8.0
// When client methods are called from outside of the Hub class, there's no caller associated with the invocation.
// Therefore, there's no access to the ConnectionId, Caller, and Others properties.
// Some kind of mapping and persistant storage is required to keep track of the connectionId and the user.

public class NotificationService(IHubContext<NotificationHub> hubContext)
{
    public async Task Send()
    {
        await hubContext.Clients.All.SendAsync("Notify", $"Something happen.");
    }
}
