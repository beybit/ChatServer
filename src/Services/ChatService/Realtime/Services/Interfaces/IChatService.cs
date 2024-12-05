using ChatService.Integration.Conversations.Dtos;

namespace ChatService.Realtime.Services.Interfaces
{
    public interface IChatService
    {
        Task<bool> SendMessageAsync(string userId, SendMessageDto message, CancellationToken cancellationToken);
    }
}
