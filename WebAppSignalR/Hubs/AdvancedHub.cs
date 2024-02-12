using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAppSignalR.Hubs;

// 1)   Strongly Typed Hub - a drawback of using SendAsync is that it relies on a string to specify the client method to be called.
//      This leaves code open to runtime errors if the method name is misspelled or missing from the client.

public interface IAdvancedHubClient
{
    Task HandleMessage(string message);
    Task HandleComplexMessage(string message);
    Task<string> WaitForMessageHandler(string connectionId);
}

// 2)   Object Parameters (API Design) - compatibility after changes, primitive parameters can cause pain
public record ChatMessage(string User, string Message);


//[Authorize]
public class AdvancedHub(ILogger<AdvancedHub> logger) : Hub<IAdvancedHubClient>
{
    //[Authorize(Roles = "Admin")]
    public async Task PrintOnConsole(string message)
    { 
        logger.LogInformation(message);

        await Clients.All.HandleMessage($"Message {message} handled");
    }

    // change the hub method name
    // [HubMethodName("SendMessageToUser")]
    public async Task<string> WaitForMessage(string connectionId)
    {
        var message = await Clients.Client(connectionId).WaitForMessageHandler(Context.ConnectionId);
        return message;
    }

    public async Task InvokeComplexType(ChatMessage chatMessage)
    {
        var message = $"Complex message received: {chatMessage.User}, {chatMessage.Message}";
        logger.LogInformation(message);
        await Clients.Caller.HandleMessage(message);
    }

    // If a HubException is thrown in a hub method, SignalR sends the entire exception message to the client, unmodified
    public Task ThrowException()
        => throw new HubException("This error will be sent to the client!");

    // Lifecycle event handlers
    public async override Task OnConnectedAsync()
    {
        logger.LogInformation("OnConnectedAsync");

        // adding current conenction to a group
        //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");

        await base.OnConnectedAsync();
    }
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        logger.LogInformation($"OnDisconnectedAsync: {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    //// [HubMethodName("SendMessageToUser")] <- change the name of the hub method
    //public async Task SendMessage(string user, string message)
    //    => await Clients.All.ReceiveMessage(user, message);

    //public async Task SendMessage(ChatMessage chatMessage)
    //    => await Clients.All.ReceiveMessage(chatMessage.User, chatMessage.Message);

    //// js/ts call: connection.invoke("SendMessage", { param1: "value1", param2: "value2" });

    //public async Task SendMessageToCaller(string user, string message)
    //    => await Clients.Caller.ReceiveMessage(user, message);

    //public async Task SendMessageToGroup(string user, string message)
    //    => await Clients.Group("SignalR Users").ReceiveMessage(user, message);
}
