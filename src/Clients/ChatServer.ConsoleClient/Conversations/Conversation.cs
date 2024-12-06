using ChatService.Integration.Users.Dtos;

namespace ChatServer.ConsoleClient.Conversations
{
    public class Conversation<T> : IConversation<T>
        where T : class
    {
        private readonly Guid _conversationId;
        private readonly T _user;

        public Conversation(Guid conversationId, T user)
        {
            _conversationId = conversationId;
            _user = user;
        }

        public Guid ConversationId { get => _conversationId; }

        public override string ToString() => _user.ToString();
    }
}
