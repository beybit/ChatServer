using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Realtime.Hubs;
using ChatService.Realtime.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Consumers
{
    public record SendConversationMessage(Guid ConversationId, Guid MessageId);

    public class SendConversationMessageConsumer(ApplicationDbContext dbContext, IChatService chatService)
        : IConsumer<SendConversationMessage>
    {
        public async Task Consume(ConsumeContext<SendConversationMessage> context)
        {
            var message = await dbContext.Messages
                .Include(x => x.Sender)
                .SingleAsync(x => x.Id == context.Message.MessageId, context.CancellationToken);

            await chatService.SendGroupMessageAsync(message.ConversationId, new SendMessageDto(
                    message.Id,
                    message.Text,
                    message.Sender.Email!,
                    message.ConversationId,
                    message.CreatedAt
                ), context.CancellationToken);
        }
    }
}
