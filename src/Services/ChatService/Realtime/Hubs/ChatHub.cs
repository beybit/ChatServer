using ChatService.Extensions;
using ChatService.Integration.Conversations.Commands;
using ChatService.Realtime.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace ChatService.Realtime.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public IChatSessionService _chatSessionService;
        private readonly IMediator _mediator;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatSessionService chatSessionService, IMediator mediator, ILogger<ChatHub> logger)
        {
            _chatSessionService = chatSessionService;
            _mediator = mediator;
            _logger = logger;
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

        public async Task JoinGroup(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
            _logger.LogDebug("User {UserId} joined group {ConversationId}", Context.UserIdentifier, conversationId);
        }

        public async Task LeaveGroup(Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
            _logger.LogDebug("User {UserId} left group {ConversationId}", Context.UserIdentifier, conversationId);
        }
    }
}
