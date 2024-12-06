using ChatService.Application.Features.Common.UserRequest;
using ChatService.Infrastructure;
using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using ChatService.Realtime.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Users.Queries.GetAll
{
    public class GetUsersQueryHandler(ApplicationDbContext dbContext, IChatSessionService chatSessionService) : IUserRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(UserRequest<GetUsersQuery, List<UserDto>> request, CancellationToken cancellationToken)
        {
            var users = await dbContext.Users
                .Where(x => x.Id != request.UserId)
                .Select(x => new
                {
                    x.Id,
                    x.Email
                })
                .ToListAsync(cancellationToken);
            var onlineUsers = await chatSessionService.GetUserSessionsAsync();

            return users.Select(x => new UserDto(x.Email!, onlineUsers.Contains(x.Id))).ToList();
        }
    }
}
