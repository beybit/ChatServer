using ChatServer.ConsoleClient;
using ChatServer.ConsoleClient.Clients;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatServer.ConsoleClient.Conversations;
using ChatServer.ConsoleClient.Authorization;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<AuthenticationHeaderHttpInterceptor>();
builder.Services.AddSingleton<AuthTokenProvider>();

builder.Services.AddHttpClient("Auth", x =>
{
    x.BaseAddress = new Uri("https://localhost:18443");
});
    
builder.Services.AddHttpClient("ChatAPI", (x) =>
{
    x.BaseAddress = new Uri("https://localhost:18443");
}).AddHttpMessageHandler<AuthenticationHeaderHttpInterceptor>();

builder.Services.AddScoped(x => x.GetRequiredService<IHttpClientFactory>().CreateClient("ChatAPI"));

builder.Services.AddScoped<UserSignInClient>();
builder.Services.AddScoped<UsersClient>();
builder.Services.AddScoped<ConversationClient>();

builder.Services.AddScoped<UserConversationService>();
builder.Services.AddSingleton<ChatApp>();

builder.Services.AddSingleton(x =>
{
    HubConnection connection = new HubConnectionBuilder()
        .WithUrl(new Uri("https://localhost:18443/chathub"), options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(x.GetRequiredService<AuthTokenProvider>().AccessToken);
        })
        .WithAutomaticReconnect()
        .Build();

    return connection;
});

using IHost host = builder.Build();
using var scope = host.Services.CreateScope();
var chatApp = scope.ServiceProvider.GetRequiredService<ChatApp>();
await chatApp.RunAsync();