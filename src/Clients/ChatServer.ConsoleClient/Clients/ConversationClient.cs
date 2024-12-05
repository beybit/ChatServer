using ChatService.Integration.Conversations.Commands;
using ChatService.Integration.Conversations.Dtos;
using System.Net.Http.Json;

namespace ChatServer.ConsoleClient.Clients
{
    public class ConversationClient(HttpClient http)
    {
        public async Task SendMessageAsync(SendMessageCommand command)
        {
            var response = await http.PostAsJsonAsync("/api/conversation/messages", command);

            response.EnsureSuccessStatusCode();
        }

        public async Task<StartConversationReply?> StartAsync(StartConversationCommand command)
        {
            var response = await http.PostAsJsonAsync("/api/conversation", command);

            return await response.ReadJson<StartConversationReply>();
        }

        public async Task MessageViewedAsync(MessageViewedCommand command)
        {
            var response = await http.PostAsJsonAsync("/api/conversation/messages/viewed", command);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ConversationMessageDto>?> GetMessagesAsync(Guid conversationId)
        {
            return await http.GetFromJsonAsync<List<ConversationMessageDto>>($"/api/conversation/messages/{conversationId}");
        }
    }
}
