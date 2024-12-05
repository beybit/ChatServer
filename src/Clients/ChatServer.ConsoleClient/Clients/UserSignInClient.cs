using ChatService.Integration.Users.Commands;
using System.Net.Http.Json;

namespace ChatServer.ConsoleClient.Clients
{
    public class UserSignInClient(IHttpClientFactory httpClientFactory)
    {
        public async Task<RegisterReply?> Register(RegisterCommand registerCommand)
        {
            var http = httpClientFactory.CreateClient("Auth");
            var response = await http.PostAsJsonAsync("/api/users/register", registerCommand);

            return await response.ReadJson<RegisterReply>();
        }

        public async Task<SignInReply?> SignIn(SignInCommand registerCommand)
        {
            var http = httpClientFactory.CreateClient("Auth");
            var response = await http.PostAsJsonAsync("/api/users/signin", registerCommand);

            return await response.ReadJson<SignInReply>();
        }
    }
}
