using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebAppSignalR.Hubs;

namespace SignalRChat.Hubs;

// SignalR hub documentation
// https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-8.0
// https://learn.microsoft.com/en-us/aspnet/core/signalr/api-design?view=aspnetcore-8.0


[Authorize]
public class ChatHub : Hub
{
    
    public async Task SendMessage(string user, string message)
    {
        // context
        var context = this.Context;
        var connectionId = context.ConnectionId;

        // for windows auth requires IUserIdProvider implementation
        var userIdentifier = context.UserIdentifier ?? "Unknown user";
        //var userIdentityName = context.User?.Identity?.Name ?? "Unknown user";

        var httpContext = context.GetHttpContext();

        // data will persist for the connection across different hub method invocations
        var items = context.Items;
        
        await Clients.All.SendAsync("ReceiveMessage", user, $" (Clients.All) {message}");

        await Clients.Caller.SendAsync("ReceiveMessage", user, $" (Clients.Caller) {message}");

        // authentication is required for this method
        // SignalR uses ClaimTypes.NameIdentifier from the ClaimsPrincipal
        await Clients.User(userIdentifier).SendAsync("ReceiveMessage", userIdentifier, $" (Clients.User) {message}");
    }

    public async Task SendMessageToCaller(string user, string message)
        => await Clients.Caller.SendAsync("ReceiveMessage", user, message);

    public async Task SendMessageToGroup(string user, string message)
        => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);

    public async Task SendMessageToUser(string user, string message)
        => await Clients.User(user).SendAsync("ReceiveMessage", user, message);


    // Groups
    // A group is a collection of connections associated with a name
    public async Task JoinGroup(string groupName) 
        => await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    public async Task LeaveGroup(string groupName)
    => await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

    public Task PingGroup(string groupName, string message) 
        => Clients.Group(groupName).SendAsync("PingHandler", message);
}


