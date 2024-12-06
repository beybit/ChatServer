using ChatServer.ConsoleClient.Clients;
using ChatService.Integration.Conversations.Commands;
using ChatService.Integration.Users.Dtos;

namespace ChatServer.ConsoleClient.Conversations
{

    public class UserConversationService : ConversationService<UserDto>
    {
        public UserConversationService(ConversationClient messagesClient) : base(messagesClient)
        {
        }

        public override async Task StartConversationAsync(string email, UserDto user)
        {
            var conversation = await ConversationClient.StartAsync(new StartConversationCommand { Email = user.Email });
            if(conversation != null)
            {
                await StartConversationInternalAsync(email, new Conversation<UserDto>(conversation.ConversationId, user));
            }
            else
            {
                // TODO
            }
        }
    }
}
