using System.Threading;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatService.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ChatService.Domain;
using NotificationService.Integration;
using Microsoft.AspNetCore.Identity;
using ChatService.Integration.Users.Commands;

namespace ChatService.Application.Features.Users.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterReply>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public RegisterCommandHandler(IPublishEndpoint publishEndpoint, UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<RegisterReply> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                user = new User
                {
                    UserName = request.Email,
                    Email = request.Email
                };
                _dbContext.Users.Add(user);
            }

            var otp = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.Email!);
            await _publishEndpoint.Publish(new SendEmail { Email = user.Email!, Subject = "Verification Code", Body = $"Your verification code: {otp}" }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new RegisterReply
            {
                Data = otp
            };
        }
    }
}
