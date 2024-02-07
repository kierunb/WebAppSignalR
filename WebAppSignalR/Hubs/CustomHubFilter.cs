using Microsoft.AspNetCore.SignalR;

namespace WebAppSignalR.Hubs;

// Hub Filters
// https://learn.microsoft.com/en-us/aspnet/core/signalr/hub-filters?view=aspnetcore-8.0


public class CustomHubFilter : IHubFilter
{
    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
    {
        Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception calling '{invocationContext.HubMethodName}': {ex}");
            throw;
        }
    }

    // Optional method
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        return next(context);
    }

    // Optional method
    public Task OnDisconnectedAsync(
        HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
    {
        return next(context, exception);
    }
}


// Usage of the filter:
// [LanguageFilter(filterArgument = 0)]

//services.AddSignalR(hubOptions =>
//    {
//        hubOptions.AddFilter<LanguageFilter>();
//    });

//services.AddSingleton<LanguageFilter>();