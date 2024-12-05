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
        public async Task<bool> SendMessageAsync(string userId, SendMessageDto message, CancellationToken cancellationToken)
        {
            logger.LogDebug("Send convesation message {MessageId} to user {UserId} attempt", message.ConversationMessageId, userId);
            var userSession = await chatSessionService.GetSessionAsync(userId);
            if(userSession == null )
            {
                logger.LogDebug("Send convesation message {MessageId} to user {UserId} failed: user is offline", message.ConversationMessageId, userId);
                return false;
            }

            await chatHubContext.Clients.User(userId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(message), cancellationToken);
            logger.LogDebug("Send convesation message {MessageId} to user {UserId} success", message.ConversationMessageId, userId);

            return true;
        }
    }
}
