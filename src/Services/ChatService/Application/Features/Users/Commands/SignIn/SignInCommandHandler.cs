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
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using ChatService.Integration.Users.Commands;

namespace ChatService.Application.Features.Users.Commands.SignIn
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInReply>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public SignInCommandHandler(IPublishEndpoint publishEndpoint, UserManager<User> userManager,
            IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<SignInReply> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                throw new Exception($"User with email {request.Email} not found!");
            }

            var isOtpValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, request.Otp, user.Email!);

            if (isOtpValid)
            {
                var claims = new List<Claim> {
                    new(ClaimTypes.Name, user.Email!),
                    new(ClaimTypes.NameIdentifier, user.Id)
                };
                var jwt = new JwtSecurityToken(
                        issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                SecurityAlgorithms.HmacSha256));

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

                return new SignInReply
                {
                    AccessToken = accessToken,
                };
            }
            else
            {
                throw new InvalidOperationException($"Invlaid OTP code");
            }
        }
    }
}
