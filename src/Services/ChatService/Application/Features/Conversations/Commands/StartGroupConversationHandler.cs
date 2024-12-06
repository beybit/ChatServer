using ChatService.Application.Features.Common.UserRequest;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Commands
{
    public class StartGroupConversationHandler(ApplicationDbContext dbContext)
        : IUserRequestHandler<StartGroupConversationCommand, StartConversationReply>
    {
        public async Task<StartConversationReply> Handle(UserRequest<StartGroupConversationCommand, StartConversationReply> request, CancellationToken cancellationToken)
        {
            var conversationId = await (from c in dbContext.Conversations
                                      join g in dbContext.ChatGroups on c.Id equals g.ConversationId
                                      where g.Id == request.Params.GroupId
                                      select c.Id)
                                      .SingleAsync(cancellationToken);

            if (! await dbContext.ConversationUsers.AnyAsync(x => x.ConversationId == conversationId && x.UserId == request.UserId))
            {
                var conversationUser = new ConversationUser
                {
                    Id = NewId.NextSequentialGuid(),
                    CreatedAt = DateTimeOffset.Now,
                    UserId = request.UserId,
                    ConversationId = conversationId
                };
                dbContext.ConversationUsers.Add(conversationUser);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return new StartConversationReply
            {
                ConversationId = conversationId
            };
        }
    }
}
