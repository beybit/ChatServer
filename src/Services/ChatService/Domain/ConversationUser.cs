namespace ChatService.Domain
{
    public class ConversationUser
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid ConversationId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; }

        public Conversation Conversation { get; set; }
    }
}
