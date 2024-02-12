using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Hello, .NET SignalR Client!");


var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7003/advancedHub")
    .Build();

connection.On<string>("HandleMessage", (message) =>
{
    Console.WriteLine($"Server: {message}");
});


await connection.StartAsync();

await connection.InvokeAsync("PrintOnConsole", "Hello, SignalR Server!");

await connection.StopAsync();