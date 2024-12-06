using ChatService.Application.Features.Common.UserRequest;
using ChatService.Extensions;
using ChatService.Integration.Groups.Commands;
using ChatService.Integration.Groups.Dtos;
using ChatService.Integration.Groups.Queries;
using ChatService.Integration.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers
{
    [Route("api/chat-groups")]
    [Authorize]
    public class ChatGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet()]
        public Task<List<ChatGroupDto>> GetAll()
             => _mediator.SendHttpContext<GetChatGroupsQuery, List<ChatGroupDto>>(HttpContext, new GetChatGroupsQuery());

        [HttpPost]
        public async Task<ChatGroupDto> Create(CreateChatGroupCommand command)
             => await _mediator.SendHttpContext<CreateChatGroupCommand, ChatGroupDto>(HttpContext, command);
    }
}
