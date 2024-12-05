using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Realtime.Hubs;
using ChatService.Realtime.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Consumers
{
    public record SendConversationUserMessage(Guid ConversationMessageId);

    public class SendConversationUserMessageConsumer(ApplicationDbContext dbContext, IChatService chatService)
        : IConsumer<SendConversationUserMessage>
    {
        public async Task Consume(ConsumeContext<SendConversationUserMessage> context)
        {
            var conversataionMessage = await dbContext.ConversationMessages
                .Include(x => x.ConversationUser)
                    .ThenInclude(x => x.User)
                .Include(x => x.Message)
                .SingleAsync(x => x.Id == context.Message.ConversationMessageId, context.CancellationToken);
            if (conversataionMessage.Status == Domain.ConversationMessageStatus.Created)
            {
                var isSent = await chatService.SendMessageAsync(conversataionMessage.ConversationUser.UserId, new SendMessageDto(
                    conversataionMessage.Id,
                    conversataionMessage.Message.Text,
                    conversataionMessage.ConversationUser.User.Email!,
                    conversataionMessage.ConversationUser.ConversationId,
                    conversataionMessage.Message.CreatedAt
                ), context.CancellationToken);

                if (isSent)
                {
                    conversataionMessage.Status = Domain.ConversationMessageStatus.Sent;
                    await dbContext.SaveChangesAsync(context.CancellationToken);
                }
            }
        }
    }
}
