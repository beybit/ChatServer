using ChatService.Domain;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Realtime.Hubs;
using ChatService.Realtime.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ChatService.Realtime.Services
{
    public class ChatService(IHubContext<ChatHub> chatHubContext, IChatSessionService chatSessionService, ILogger<ChatService> logger) : IChatService
    {
        public async Task<bool> SendGroupMessageAsync(Guid conversationId, SendMessageDto message, CancellationToken cancellationToken)
        {
            logger.LogDebug("Send message {MessageId} to group conversation {ConversationId} attempt", message.MessageId, conversationId);
            await chatHubContext.Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", JsonSerializer.Serialize(message), cancellationToken);
            logger.LogDebug("Send message {MessageId} to group conversation {ConversationId} success", message.MessageId, conversationId);

            return true;
        }

        public async Task<bool> SendMessageAsync(string userId, SendMessageDto message, CancellationToken cancellationToken)
        {
            logger.LogDebug("Send conversation message {MessageId} to user {UserId} attempt", message.MessageId, userId);
            var userSession = await chatSessionService.GetSessionAsync(userId);
            if(userSession == null )
            {
                logger.LogDebug("Send conversation message {MessageId} to user {UserId} failed: user is offline", message.MessageId, userId);
                return false;
            }

            await chatHubContext.Clients.User(userId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(message), cancellationToken);
            logger.LogDebug("Send conversation message {MessageId} to user {UserId} success", message.MessageId, userId);

            return true;
        }
    }
}
