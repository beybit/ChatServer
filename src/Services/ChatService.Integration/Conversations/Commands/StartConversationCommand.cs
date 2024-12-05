using MediatR;

namespace ChatService.Integration.Conversations.Commands
{
    public class StartConversationCommand : IRequest<StartConversationReply>
    {
        public required string Email { get; set; }
    }
    public class StartConversationReply
    {
        public Guid ConversationId { get; set; }
    }
}
