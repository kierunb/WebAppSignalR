using Microsoft.AspNetCore.SignalR;
using WebAppSignalR.Hubs;

namespace SignalRChat.Hubs;

// SignalR hub documentation
// https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-8.0
// https://learn.microsoft.com/en-us/aspnet/core/signalr/api-design?view=aspnetcore-8.0

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // context
        var context = this.Context;
        var connectionId = context.ConnectionId;
        var userIdentifier = context.UserIdentifier;
        
        await Clients.All.SendAsync("ReceiveMessage", user, $" (Clients.All) {message}");

        await Clients.Caller.SendAsync("ReceiveMessage", user, $" (Clients.Caller) {message}");

        // authentication is required for this method
        // SignalR uses ClaimTypes.NameIdentifier from the ClaimsPrincipal
        await Clients.User(user).SendAsync("ReceiveMessage", user, $" (Clients.User) {message}");
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


// 1)   Strongly Typed Hub - a drawback of using SendAsync is that it relies on a string to specify the client method to be called.
//      This leaves code open to runtime errors if the method name is misspelled or missing from the client.

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}

// 2)   Object Parameters (API Design) - compatibility after changes, primitive parameters can cause pain
public record ChatMessage(string User, string Message);


public class StronglyTypedChatHub : Hub<IChatClient>
{

    // [HubMethodName("SendMessageToUser")] <- change the name of the hub method
    public async Task SendMessage(string user, string message)
        => await Clients.All.ReceiveMessage(user, message);

    public async Task SendMessage(ChatMessage chatMessage)
        => await Clients.All.ReceiveMessage(chatMessage.User, chatMessage.Message);

    // js/ts call: connection.invoke("SendMessage", { param1: "value1", param2: "value2" });

    public async Task SendMessageToCaller(string user, string message)
        => await Clients.Caller.ReceiveMessage(user, message);

    public async Task SendMessageToGroup(string user, string message)
        => await Clients.Group("SignalR Users").ReceiveMessage(user, message);
}


