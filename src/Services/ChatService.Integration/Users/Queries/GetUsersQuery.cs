using ChatService.Integration.Users.Dtos;
using MediatR;

namespace ChatService.Integration.Users.Queries
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
}
