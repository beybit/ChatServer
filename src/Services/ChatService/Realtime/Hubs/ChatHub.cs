using ChatService.Extensions;
using ChatService.Realtime.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace ChatService.Realtime.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public IChatSessionService _chatSessionService;

        public ChatHub(IChatSessionService chatSessionService)
        {
            _chatSessionService = chatSessionService;
        }

        public override async Task OnConnectedAsync()
        {
            await _chatSessionService.StartAsync(Context.User!.GetUserId(), Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _chatSessionService.StopAsync(Context.User!.GetUserId());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
