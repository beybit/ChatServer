using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Conversations.Commands
{
    public class SendMessageCommand : IRequest
    {
        public required string Text { get; set; }

        public required Guid ConversationId { get; set; }
    }
    public class MessageViewedCommand : IRequest
    {
        public required Guid ConversationMessageId { get; set; }
    }
}
