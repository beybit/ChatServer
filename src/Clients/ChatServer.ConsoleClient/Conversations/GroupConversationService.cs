using ChatServer.ConsoleClient.Clients;
using ChatService.Integration.Conversations.Commands;
using ChatService.Integration.Groups.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatServer.ConsoleClient.Conversations
{
    public class GroupConversationService : ConversationService<ChatGroupDto>
    {
        private readonly HubConnection _hubConnection;

        public GroupConversationService(ConversationClient messagesClient, HubConnection hubConnection) : base(messagesClient)
        {
            _hubConnection = hubConnection;
        }

        public override async Task StartConversationAsync(string email, ChatGroupDto group)
        {
            var conversation = await ConversationClient.StartAsync(new StartGroupConversationCommand { GroupId = group.Id });
            if (conversation != null)
            {
                await _hubConnection.InvokeAsync("JoinGroup", conversation.ConversationId);
                await StartConversationInternalAsync(email, new Conversation<ChatGroupDto>(conversation.ConversationId, group));
                await _hubConnection.InvokeAsync("LeaveGroup", conversation.ConversationId);
            }
            else
            {
                // TODO
            }
        }
    }
}
