using ChatService.Integration.Users.Dtos;

namespace ChatServer.ConsoleClient.Conversations
{
    public class Conversation : IConversation<UserDto>
    {
        private readonly Guid _conversationId;
        private readonly UserDto _user;

        public Conversation(Guid conversationId, UserDto user)
        {
            _conversationId = conversationId;
            _user = user;
        }

        public Guid ConversationId { get => _conversationId; }

        public override string ToString() => _user.ToString();
    }
}
