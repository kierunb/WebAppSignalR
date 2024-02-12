using Coravel;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using WebAppSignalR.Hubs;
using WebAppSignalR.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();


// If JavaScript code is hosted on a different domain from the SignalR app,
// CORS middleware must be enabled to allow the JavaScript to connect to the SignalR app
#region CORS configuration
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      policy =>
//                      {
//                          policy.WithOrigins("http://example.com");
//                          policy.WithMethods("GET", "POST");
//                          policy.AllowCredentials();
//                      });
//}); 
#endregion

// Scheduler
builder.Services.AddScheduler();
builder.Services.AddTransient<TestJob>();

// SignalR
builder.Services.AddSignalR(hubOptions =>
{
    // global filter registration
    hubOptions.AddFilter<CustomHubFilter>();
}); //.AddMessagePackProtocol(); // message pack protocol -> works for msgpack and json clients 

builder.Services.AddSingleton<CustomHubFilter>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

var app = builder.Build();


#region Scheduler Jobs
// https://docs.coravel.net/Scheduler/

app.Services.UseScheduler(scheduler =>
{
    //scheduler.Schedule(
    //    () => Console.WriteLine(">> Scheduler minute ping.")
    //)
    //.EveryMinute();

    scheduler.Schedule<TestJob>().EverySeconds(5);// .EveryMinute();
}); 
#endregion


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<AdvancedHub>("/advancedHub");

app.Run();
