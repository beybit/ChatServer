using ChatService.Realtime.Models;
using ChatService.Realtime.Services.Interfaces;
using Quartz;
using StackExchange.Redis;
using System.Text.Json;

namespace ChatService.Realtime.Services
{
    public class ChatSessionService : IChatSessionService
    {
        IDatabase _cacheDatabase;
        private readonly ILogger<ChatSessionService> _logger;
        const string OnlineSetKey = "OnlineUsers";

        public ChatSessionService(IDatabase cacheDatabase, ILogger<ChatSessionService> logger)
        {
            _cacheDatabase = cacheDatabase;
            _logger = logger;
        }

        public async Task StartAsync(string userId, string sessionId)
        {
            var chatSession = new UserChatSession(userId, sessionId);
            await _cacheDatabase.StringSetAsync(userId, JsonSerializer.Serialize(chatSession));
            await _cacheDatabase.SetAddAsync(OnlineSetKey, userId);
            _logger.LogDebug("User {UserId} chat session {SessionId} started", userId, sessionId);
        }

        public async Task StopAsync(string userId)
        {            
            await _cacheDatabase.KeyDeleteAsync(userId);
            await _cacheDatabase.SetRemoveAsync(OnlineSetKey, userId);
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

        public async Task<string[]> GetUserSessionsAsync()
        {
            var onlineUsers = await _cacheDatabase.SetMembersAsync(OnlineSetKey);
            if (onlineUsers == null) return null;
            return Array.ConvertAll(onlineUsers, x => (string)x);
        }
    }
}
