using ChatService.Application.Features.Common.UserRequest;
using ChatService.Infrastructure;
using ChatService.Integration.Groups.Dtos;
using ChatService.Integration.Groups.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Groups.Queries
{
    public class GetGroupsQueryHandler(ApplicationDbContext dbContext)
        : IUserRequestHandler<GetChatGroupsQuery, List<ChatGroupDto>>
    {
        public async Task<List<ChatGroupDto>> Handle(UserRequest<GetChatGroupsQuery, List<ChatGroupDto>> request, CancellationToken cancellationToken)
        {
            return await dbContext.ChatGroups.ProjectToDto().ToListAsync(cancellationToken);
        }
    }
}
