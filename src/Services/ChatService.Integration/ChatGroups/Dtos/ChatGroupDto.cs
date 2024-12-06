using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Groups.Dtos
{
    public record ChatGroupDto(Guid Id, string Name, Guid ConversationId)
    {
        public override string ToString() => Name;
    }
}
