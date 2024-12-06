using ChatService.Application.Features.Common.UserRequest;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Integration.Groups.Commands;
using ChatService.Integration.Groups.Dtos;
using MassTransit;

namespace ChatService.Application.Features.Groups.Commands
{
    public class CreateChatGroupCommandHandler(ApplicationDbContext dbContext)
        : IUserRequestHandler<CreateChatGroupCommand, ChatGroupDto>
    {
        public async Task<ChatGroupDto> Handle(UserRequest<CreateChatGroupCommand, ChatGroupDto> request, CancellationToken cancellationToken)
        {
            var chatGroupConversation = new Conversation
            {
                Id = NewId.NextSequentialGuid(),
                CreatedAt = DateTimeOffset.Now,                
            };
            var chatGroup = new ChatGroup
            {
                Id = NewId.NextSequentialGuid(),
                Name = request.Params.Name,
                CreatedAt = DateTimeOffset.Now,
                CreatedById = request.UserId,
                ConversationId = chatGroupConversation.Id
            };
            dbContext.Conversations.Add(chatGroupConversation);
            dbContext.ChatGroups.Add(chatGroup);
            await dbContext.SaveChangesAsync(cancellationToken);

            return chatGroup.ToDto();
        }
    }
}
