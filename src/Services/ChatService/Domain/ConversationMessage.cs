namespace ChatService.Domain
{
    public class ConversationMessage
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid ConversationUserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ConversationMessageStatus Status { get; set; }

        public Message Message { get; set; }

        public ConversationUser ConversationUser { get; set; }
    }
}
