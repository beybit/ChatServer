using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Realtime.Hubs;
using ChatService.Realtime.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Integration;

namespace ChatService.Application.Features.Conversations.Consumers
{
    public record SendMessageEmail(Guid ConversationMessageId);

    public class SendMessageEmailConsumer(ApplicationDbContext dbContext)
        : IConsumer<SendMessageEmail>
    {
        public async Task Consume(ConsumeContext<SendMessageEmail> context)
        {
            var conversataionMessage = await dbContext.ConversationMessages
                .Include(x => x.ConversationUser)
                    .ThenInclude(x => x.User)
                .Include(x => x.Message)
                    .ThenInclude(x => x.Sender)
                .SingleAsync(x => x.Id == context.Message.ConversationMessageId, context.CancellationToken);
            if (conversataionMessage.Status == Domain.ConversationMessageStatus.Created)
            {
                await context.Publish(new SendEmail
                {
                    MessageId = conversataionMessage.Id,
                    Email = conversataionMessage.ConversationUser.User.Email!,
                    Subject = $"New message from {conversataionMessage.Message.Sender.Email}",
                    Body = conversataionMessage.Message.Text
                });

                conversataionMessage.Status = Domain.ConversationMessageStatus.Sent;
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
