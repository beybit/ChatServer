using ChatService.Application.Features.Common.UserRequest;
using ChatService.Infrastructure;
using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Users.Queries.GetAll
{
    public class GetUsersQueryHandler(ApplicationDbContext dbContext) : IUserRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(UserRequest<GetUsersQuery, List<UserDto>> request, CancellationToken cancellationToken)
        {
            return await dbContext.Users
                .Where(x => x.Id != request.UserId)
                .Select(x => new UserDto
                {
                    Email = x.Email!
                })
                .ToListAsync(cancellationToken);
        }
    }
}
