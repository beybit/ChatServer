using ChatService.Application.Features.Common.UserRequest;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Commands
{
    public class MessageViewedCommandHandler(ApplicationDbContext dbContext) : IUserRequestHandler<MessageViewedCommand>
    {
        public async Task Handle(UserRequest<MessageViewedCommand> request, CancellationToken cancellationToken)
        {
            var conversationMessage = await dbContext.ConversationMessages
                .Where(x => x.Id == request.Params.ConversationMessageId && x.ConversationUser.UserId == request.UserId)
                .SingleOrDefaultAsync(cancellationToken);
            if(conversationMessage != null)
            {
                conversationMessage.Status = ConversationMessageStatus.Seen;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
