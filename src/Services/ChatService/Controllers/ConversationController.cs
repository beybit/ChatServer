using ChatService.Application.Features.Common.UserRequest;
using ChatService.Extensions;
using ChatService.Integration.Conversations.Commands;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Integration.Conversations.Queries;
using ChatService.Integration.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("messages/{conversationId}")]
        public Task<List<ConversationMessageDto>> GetMessages(Guid conversationId)
             => _mediator.SendHttpContext<GetConversationMessagesQuery, List<ConversationMessageDto>>(HttpContext, new GetConversationMessagesQuery { ConversationId = conversationId });

        [HttpPost]
        public async Task<StartConversationReply> Start(StartConversationCommand command)
             => await _mediator.SendHttpContext<StartConversationCommand, StartConversationReply>(HttpContext, command);

        [HttpPost("messages")]
        public Task SendMessage(SendMessageCommand command)
             => _mediator.SendHttpContext(HttpContext, command);

        [HttpPost("messages/viewed")]
        public Task MessageViewed(MessageViewedCommand command)
             => _mediator.SendHttpContext(HttpContext, command);
    }
}
