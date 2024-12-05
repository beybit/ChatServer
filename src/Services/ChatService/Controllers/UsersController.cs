using ChatService.Application.Features.Common;
using ChatService.Application.Features.Common.UserRequest;
using ChatService.Extensions;
using ChatService.Integration.Users.Commands;
using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatService.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet]
        public Task<List<UserDto>> GetAll([FromQuery]GetUsersQuery query)
             => _mediator.SendHttpContext<GetUsersQuery, List<UserDto>>(HttpContext, query);

        [AllowAnonymous]
        [HttpPost("register")]
        public Task<RegisterReply> RegisterAsync(RegisterCommand command)
             => _mediator.Send(command);
        
        [AllowAnonymous]
        [HttpPost("signin")]
        public Task<SignInReply> SignIn(SignInCommand command)
             => _mediator.Send(command);
    }
}
