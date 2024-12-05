namespace ChatServer.ConsoleClient.Conversations
{
    public interface IConversation<T>
        where T : class
    {
        Guid ConversationId { get; }
    }
}
