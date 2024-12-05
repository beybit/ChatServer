using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatService.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new Exception("UserId claim not found");
    }
}
