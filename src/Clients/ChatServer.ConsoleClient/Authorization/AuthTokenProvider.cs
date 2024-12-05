using ChatServer.ConsoleClient.Clients;
using ChatService.Integration.Users.Commands;
using Microsoft.Extensions.Options;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace ChatServer.ConsoleClient.Authorization
{
    public class AuthTokenProvider
    {
        private readonly UserSignInClient _usersClient;

        public AuthTokenProvider(UserSignInClient usersClient)
        {
            _usersClient = usersClient;
        }

        public string? AccessToken { get; private set; }

        public async Task<string> SignInAsync()
        {
            string? email;
            do
            {
                email = AnsiConsole.Prompt(
    new TextPrompt<string>("Hi, enter your valid email to start chat:"));
            } while (string.IsNullOrEmpty(email) || !Regex.IsMatch(email!, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}"));

            var registerReply = await _usersClient.Register(new RegisterCommand
            {
                Email = email!
            });

            SignInReply? signInReply;

            do
            {
                var otp = !string.IsNullOrEmpty(registerReply.Data) ? registerReply.Data : AnsiConsole.Prompt(
    new TextPrompt<string>($"Enter vlaid OTP code sent to your email {email}: ").Secret('*'));

                signInReply = await _usersClient.SignIn(new SignInCommand
                {
                    Email = email!,
                    Otp = otp
                });
            } while (signInReply == null);

            AnsiConsole.Clear();
            AccessToken = signInReply.AccessToken;

            return email;
        }
    }

    public class AuthenticationHeaderHttpInterceptor(AuthTokenProvider authTokenProvider) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authTokenProvider.AccessToken);

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
