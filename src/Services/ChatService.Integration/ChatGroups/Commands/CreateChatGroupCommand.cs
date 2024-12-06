using ChatService.Integration.Groups.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Groups.Commands
{
    public record CreateChatGroupCommand(string Name) : IRequest<ChatGroupDto>;
}
