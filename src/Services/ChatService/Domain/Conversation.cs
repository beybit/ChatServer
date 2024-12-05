namespace ChatService.Domain
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ChatGroup? ChatGroup { get; set; }

        public List<ConversationUser> ConversationUsers { get; set; }

        public List<ConversationMessage> Messages { get; set; }
    }
}
