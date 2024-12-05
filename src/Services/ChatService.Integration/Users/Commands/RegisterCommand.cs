using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Integration.Users.Commands
{
    public class RegisterCommand : IRequest<RegisterReply>
    {
        public string Email { get; set; }
    }
    public class RegisterReply
    {
        public string Data { get; set; }
    }
}
