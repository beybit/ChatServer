namespace ChatService.Domain
{
    public class Message
    {
        public Guid Id { get; set; }

        public required string Text { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string SenderId { get; set; }

        public User Sender { get; set; }

        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }
    }
}
