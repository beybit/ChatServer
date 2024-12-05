using ChatServer.ConsoleClient.Authorization;
using ChatServer.ConsoleClient.Clients;
using ChatServer.ConsoleClient.Conversations;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatServer.ConsoleClient
{
    public record MenuItem<T>(T Value, string title) where T : Enum
    {
        public override string ToString() => title;
    }
    public enum MainMenu { ViewUsers, ViewGroups, Exit }

    public class ChatApp
    {
        private readonly UsersClient _usersClient;
        private readonly AuthTokenProvider _authTokenProvider;
        private HubConnection _hubConnection;
        private readonly IServiceProvider _serviceProvider;
        private UserConversationService? _currentUserConversation;
        private string _email;

        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            
        };

        public ChatApp(UsersClient usersClient, AuthTokenProvider authTokenProvider, HubConnection hubConnection, IServiceProvider serviceProvider)
        {
            _usersClient = usersClient;
            _authTokenProvider = authTokenProvider;
            _hubConnection = hubConnection;
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync()
        {
            _email = await SignIn();
            await ConnectToHub();
            await MainMenuChoice();
        }

        private async Task ConnectToHub()
        {
            _hubConnection.On<string>("ReceiveMessage", (messageData) =>
            {
                var message = JsonSerializer.Deserialize<SendMessageDto>(messageData);
                _currentUserConversation?.ReceiveMessageAsync(message);
            });

            await _hubConnection.StartAsync();
        }

        private async Task MainMenuChoice()
        {
            MenuItem<MainMenu> choice;
            do
            {
                choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuItem<MainMenu>>()
                        .Title("Choose action?")
                        .AddChoices(new[] {
                            new MenuItem<MainMenu>(MainMenu.ViewUsers, "View users"),
                            new MenuItem<MainMenu>(MainMenu.ViewGroups, "View groups"),
                            new MenuItem<MainMenu>(MainMenu.Exit, "Exit"),
                        }));

                if(choice.Value == MainMenu.ViewUsers)
                {
                    await ViewUsers();
                }

            } while (choice.Value != MainMenu.Exit);
        }

        private async Task ViewUsers()
        {
            try
            {
                var users = await _usersClient.GetAll(new GetUsersQuery());

                if (users != null)
                {
                    if (users.Count == 0)
                    {
                        AnsiConsole.WriteLine("There is no users yet");
                    }

                    UserDto? choice;
                    do
                    {
                        choice = AnsiConsole.Prompt(
                            new SelectionPrompt<UserDto>()
                                .Title("Choose user for conversation?")
                                .AddChoices(users));

                        await StartConversation(choice);

                    } while (choice != null);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("View users error: {0}", ex.Message);
            }
        }

        private async Task StartConversation(UserDto user)
        {
            using var scope = _serviceProvider.CreateScope();
            _currentUserConversation = scope.ServiceProvider.GetRequiredService<UserConversationService>();
            await _currentUserConversation.StartConversationAsync(_email, user);
            _currentUserConversation = null;
        }

        private async Task<string> SignIn()
        {
            return await _authTokenProvider.SignInAsync();
        }
    }
}
