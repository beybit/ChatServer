using ChatService.Application.Features.Common.UserRequest;
using ChatService.Infrastructure;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Integration.Conversations.Queries;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversations.Queries.Messages
{
    public class GetConversationMessageQueryHandler(ApplicationDbContext dbContext) : IUserRequestHandler<GetConversationMessagesQuery, List<ConversationMessageDto>>
    {
        public async Task<List<ConversationMessageDto>> Handle(UserRequest<GetConversationMessagesQuery, List<ConversationMessageDto>> request, CancellationToken cancellationToken)
        {
            return await dbContext.Messages
                .Where(x => x.ConversationId == request.Params.ConversationId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new ConversationMessageDto(x.Id, x.Text, x.Sender.Email!, x.ConversationId, x.CreatedAt))
                .ToListAsync(cancellationToken);                
        }
    }
}
