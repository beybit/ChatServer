using ChatService.Application.Features.Common.UserRequest;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Commands
{
    public class StartConversationHandler(ApplicationDbContext dbContext)
        : IUserRequestHandler<StartConversationCommand, StartConversationReply>
    {
        public async Task<StartConversationReply> Handle(UserRequest<StartConversationCommand, StartConversationReply> request, CancellationToken cancellationToken)
        {
            var conversationUserId = await dbContext.Users
                .Where(x => x.Email == request.Params.Email)
                .Select(x => x.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(conversationUserId))
            {
                throw new InvalidOperationException($"User with email {request.Params.Email} not found");
            }

            var conversationId = await (from cu1 in dbContext.ConversationUsers
                                        join cu2 in dbContext.ConversationUsers on cu1.ConversationId equals cu2.ConversationId
                                        where cu1.UserId == request.UserId && cu2.UserId == conversationUserId
                                        select (Guid?)cu1.ConversationId)
                            .FirstOrDefaultAsync(cancellationToken);

            if (conversationId == null)
            {
                var conversation = new Conversation
                {
                    Id = NewId.NextSequentialGuid(),
                    CreatedAt = DateTimeOffset.Now,
                };
                dbContext.Conversations.Add(conversation);

                dbContext.ConversationUsers.Add(new ConversationUser
                {
                    Id = NewId.NextSequentialGuid(),
                    CreatedAt = DateTimeOffset.Now,
                    ConversationId = conversation.Id,
                    UserId = request.UserId
                });
                dbContext.ConversationUsers.Add(new ConversationUser
                {
                    Id = NewId.NextSequentialGuid(),
                    CreatedAt = DateTimeOffset.Now,
                    ConversationId = conversation.Id,
                    UserId = conversationUserId
                });

                await dbContext.SaveChangesAsync(cancellationToken);
                conversationId = conversation.Id;
            }

            return new StartConversationReply
            {
                ConversationId = conversationId.Value
            };
        }
    }
}
