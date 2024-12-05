using ChatService.Integration.Conversations.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Conversations.Queries
{
    public class GetConversationMessagesQuery : IRequest<List<ConversationMessageDto>>
    {
        public Guid ConversationId { get; set; }
    }
}
