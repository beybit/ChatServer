using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Conversations.Dtos
{
    public record SendMessageDto(Guid ConversationMessageId, string Message, string FromEmail, Guid FromConversationId, DateTimeOffset CreatedAt);
}
