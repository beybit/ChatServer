namespace ChatService.Integration.Conversations.Dtos
{
    public record ConversationMessageDto(Guid Id, string Text, string FromEmail, Guid ConversationId, DateTimeOffset CreatedAt);
}
