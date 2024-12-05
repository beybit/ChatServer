namespace ChatService.Domain
{
    public class ChatGroup
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string CreatedById { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public User CreatedBy { get; set; }

        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }
    }
}
