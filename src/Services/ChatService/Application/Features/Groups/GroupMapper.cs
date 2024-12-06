using ChatService.Domain;
using ChatService.Integration.Groups.Dtos;
using Riok.Mapperly.Abstractions;

namespace ChatService.Application.Features.Groups
{
    [Mapper]
    public static partial class GroupMapper
    {
        public static partial ChatGroupDto ToDto(this ChatGroup user);

        public static partial IQueryable<ChatGroupDto> ProjectToDto(this IQueryable<ChatGroup> q);
    }
}
