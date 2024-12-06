using ChatServer.ConsoleClient.Authorization;
using ChatServer.ConsoleClient.Clients;
using ChatServer.ConsoleClient.Conversations;
using ChatService.Integration.Conversations.Dtos;
using ChatService.Integration.Groups.Dtos;
using ChatService.Integration.Groups.Queries;
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
        private IConversationSerice? _currentUserConversation;
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
                } else if(choice.Value == MainMenu.ViewGroups)
                {
                    await ViewGroups();
                }

            } while (choice.Value != MainMenu.Exit);
        }

        private async Task ViewGroups()
        {
            try
            {
                var groups = await _usersClient.GetGroups(new GetChatGroupsQuery());

                if (groups != null)
                {
                    PromptChoice<ChatGroupDto>? choice;
                    do
                    {
                        if (groups.Count == 0)
                        {
                            AnsiConsole.WriteLine("There is no groups yet");
                        }
                        var groupsPrompt = new SelectionPrompt<PromptChoice<ChatGroupDto>>()
                                .Title("Choose group for conversation?")
                                .AddChoices(new PromptChoice<ChatGroupDto>("[[< Back]]"), new PromptChoice<ChatGroupDto>("[[+ Create group]]"))
                                .AddChoices(groups.Select(x => new PromptChoice<ChatGroupDto>(x)));

                        choice = AnsiConsole.Prompt(groupsPrompt);

                        if (choice != null)
                        {
                            if(choice.ChoiceType == PromptChoiceType.Value)
                            {
                                await StartGroupConversation(choice.Value!);
                            } 
                            else if (choice.ChoiceType == PromptChoiceType.Command)
                            {
                                if(choice.CommandText == "[[+ Create group]]")
                                {
                                    AnsiConsole.Clear();
                                    var group = await CreateGroupAsync();
                                    if(group != null)
                                    {
                                        groups.Add(group);
                                    }
                                }
                            }
                        }

                    } while (choice != null && choice.ChoiceType == PromptChoiceType.Value);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("View users error: {0}", ex.Message);
            }
        }

        private async Task<ChatGroupDto?> CreateGroupAsync()
        {
            var groupName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new group name:").AllowEmpty());

            if(!string.IsNullOrEmpty(groupName))
            {
                var group = await _usersClient.CreateGroup(groupName);
                if (group != null)
                {
                    await StartGroupConversation(group);
                }
                return group;
            }
            return null;
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

                    PromptChoice<UserDto>? choice;
                    do
                    {
                        choice = AnsiConsole.Prompt(
                            new SelectionPrompt<PromptChoice<UserDto>>()
                                .Title("Choose user for conversation?")
                                .AddChoices(new PromptChoice<UserDto>("[[<- Back]]"))
                                .AddChoices(users.Select(x => new PromptChoice<UserDto>(x))));

                        if(choice != null && choice.ChoiceType == PromptChoiceType.Value)
                        {
                            await StartUserConversation(choice.Value!);
                        }

                    } while (choice != null && choice.ChoiceType == PromptChoiceType.Value);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("View users error: {0}", ex.Message);
            }
        }

        private async Task StartUserConversation(UserDto user)
        {
            using var scope = _serviceProvider.CreateScope();
            var userConversation = scope.ServiceProvider.GetRequiredService<UserConversationService>();
            _currentUserConversation = userConversation;
            await userConversation.StartConversationAsync(_email, user);
            _currentUserConversation = null;
        }

        private async Task StartGroupConversation(ChatGroupDto  group)
        {
            using var scope = _serviceProvider.CreateScope();
            var groupConversation = scope.ServiceProvider.GetRequiredService<GroupConversationService>();
            _currentUserConversation = groupConversation;
            await groupConversation.StartConversationAsync(_email, group);
            _currentUserConversation = null;
        }

        private async Task<string> SignIn()
        {
            return await _authTokenProvider.SignInAsync();
        }
    }

    public class PromptChoice<T>
    {
        private PromptChoiceType _choiceType;
        private string? _commandText;
        private T? _value;

        public PromptChoice(T value)
        {
            _choiceType = PromptChoiceType.Value;
            _value = value;
        }
        public PromptChoice(string commandText)
        {
            _choiceType = PromptChoiceType.Command;
            _commandText = commandText;
        }

        public PromptChoiceType ChoiceType { get => _choiceType; }

        public string CommandText { get => _commandText ?? _value!.ToString()!; }
        public T? Value { get => _value; }

        public override string? ToString() => CommandText;
    }

    public enum PromptChoiceType
    {
        Command = 0,
        Value = 1
    }
}
