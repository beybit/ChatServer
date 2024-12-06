using ChatService.Application.Features.Common.UserRequest;
using ChatService.Application.Features.Conversations.Consumers;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Commands
{

    public partial class SendMessageHandler(ApplicationDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            ILogger<SendMessageHandler> logger)
        : IUserRequestHandler<SendMessageCommand>
    {
        public async Task Handle(UserRequest<SendMessageCommand> request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Send message {@Message} command from user {UserId} started", request.Params, request.UserId);
            var message = new Message
            {
                Id = NewId.NextSequentialGuid(),
                ConversationId = request.Params.ConversationId,
                CreatedAt = DateTimeOffset.Now,
                Text = request.Params.Text,
                SenderId = request.UserId
            };

            dbContext.Messages.Add(message);

            var conversation = await dbContext.Conversations.SingleAsync(x => x.Id == request.Params.ConversationId, cancellationToken);
            var conversationUsers = await dbContext.ConversationUsers
                .Where(x => x.UserId != request.UserId && x.ConversationId == request.Params.ConversationId)
                .Select(x => new
                {
                    ConversationUserId = x.Id
                })
                .ToListAsync(cancellationToken);

            var conversationMessages = conversationUsers
                            .Select(x => new ConversationMessage
                            {
                                Id = NewId.NextSequentialGuid(),
                                ConversationUserId = x.ConversationUserId,
                                MessageId = message.Id,
                                Status = ConversationMessageStatus.Created,
                                CreatedAt = DateTimeOffset.UtcNow
                            })
                            .ToList();
            await dbContext.ConversationMessages.AddRangeAsync(conversationMessages);

            if(await dbContext.ChatGroups.AnyAsync(x => x.ConversationId == request.Params.ConversationId, cancellationToken))
            {
                await publishEndpoint.Publish(new SendConversationMessage(request.Params.ConversationId, message.Id), cancellationToken);
            }
            else
            {
                await publishEndpoint.PublishBatch(conversationMessages.Select(x => new SendConversationUserMessage(x.Id)), cancellationToken);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Send message {@Message} command from user {UserId} completed", request.Params, request.UserId);
        }
    }
}
