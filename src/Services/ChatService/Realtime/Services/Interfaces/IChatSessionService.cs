using ChatService.Realtime.Models;

namespace ChatService.Realtime.Services.Interfaces
{
    public interface IChatSessionService
    {
        Task StartAsync(string userId, string sessionId);
        Task StopAsync(string sessionId);
        Task<UserChatSession?> GetSessionAsync(string userId);
    }
}
