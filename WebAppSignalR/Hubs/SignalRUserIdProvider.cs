using Microsoft.AspNetCore.SignalR;

namespace WebAppSignalR.Hubs;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Implement user mapper/resolver here
        
        return connection.GetHttpContext()?.User?.Identity?.Name ?? "UNKNOWN";
    }
}
