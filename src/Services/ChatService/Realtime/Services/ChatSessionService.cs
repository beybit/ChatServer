using ChatService.Realtime.Models;
using ChatService.Realtime.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ChatService.Realtime.Services
{
    public class ChatSessionService : IChatSessionService
    {
        IDatabase _cacheDatabase;
        private readonly ILogger<ChatSessionService> _logger;

        public ChatSessionService(IDatabase cacheDatabase, ILogger<ChatSessionService> logger)
        {
            _cacheDatabase = cacheDatabase;
            _logger = logger;
        }

        public async Task StartAsync(string userId, string sessionId)
        {
            var chatSession = new UserChatSession(sessionId, userId);
            await _cacheDatabase.StringSetAsync(userId, JsonSerializer.Serialize(chatSession));
            _logger.LogDebug("User {UserId} chat session {SessionId} started", userId, sessionId);
        }

        public async Task StopAsync(string userId)
        {
            await _cacheDatabase.KeyDeleteAsync(userId);
            _logger.LogDebug("User {UserId} chat session completed", userId);
        }

        public async Task<UserChatSession?> GetSessionAsync(string userId)
        {
            var userSessionValue = await _cacheDatabase.StringGetAsync(userId);
            if(userSessionValue.HasValue)
            {
                return JsonSerializer.Deserialize<UserChatSession>(userSessionValue!);
            }

            return null;
        }
    }
}
