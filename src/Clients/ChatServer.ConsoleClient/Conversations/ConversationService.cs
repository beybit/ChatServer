using ChatServer.ConsoleClient.Clients;
using ChatService.Integration.Conversations.Commands;
using ChatService.Integration.Conversations.Dtos;
using Microsoft.VisualBasic;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.ConsoleClient.Conversations
{
    public interface IConversationSerice
    {
        Task ReceiveMessageAsync(SendMessageDto message);
    }

    public abstract class ConversationService<T> : IConversationSerice
            where T : class
    {
        const int MESSAGE_INPUT_PADDING = 2;

        private IConversation<T> _conversation;
        private string _email;
        private readonly ConversationClient _conversationClient;

        public ConversationService(ConversationClient messagesClient)
        {
            _conversationClient = messagesClient;
        }

        public IConversation<T> Conversation { get => _conversation; }

        protected ConversationClient ConversationClient { get => _conversationClient; }

        public abstract Task StartConversationAsync(string email, T target);

        protected async Task StartConversationInternalAsync(string email, IConversation<T> conversation)
        {
            _email = email;
            _conversation = conversation;

            AnsiConsole.Write(new Rule($"[red]{_conversation}[/]").Centered());
            AnsiConsole.WriteLine("Enter message to send (or empty message to exit)");

            await LoadConversationMessages(_conversation.ConversationId);

            string? message = ".";
            do
            {
                var prompt = new TextPrompt<string>("").AllowEmpty();
                message = prompt.Show(AnsiConsole.Console);
                if(!string.IsNullOrEmpty(message))
                {
                    await SendMessageAsync(message);
                }
            } while (!string.IsNullOrEmpty(message));
        }

        private async Task SendMessageAsync(string message)
        {
            await _conversationClient.SendMessageAsync(new SendMessageCommand { ConversationId = _conversation.ConversationId, Text = message });
            AnsiConsole.Cursor.MoveUp();
            ConsoleWriteMyMessage(message, _email, DateTimeOffset.Now);
            AnsiConsole.Cursor.MoveLeft(Console.CursorLeft);
            AnsiConsole.Cursor.MoveDown();
        }

        private async Task LoadConversationMessages(Guid conversationId)
        {
            var messages = await _conversationClient.GetMessagesAsync(conversationId);

            if(messages != null)
            {
                foreach (var message in messages)
                {
                    if(message.FromEmail == _email)
                    {
                        ConsoleWriteMyMessage(message.Text, message.FromEmail, message.CreatedAt.ToLocalTime());
                    } 
                    else
                    {
                        ConsoleWriteConversationMessage(message.Text, message.FromEmail, message.CreatedAt.ToLocalTime());
                    }
                }
            }
        }

        public async Task ReceiveMessageAsync(SendMessageDto message)
        {
            if(message.FromConversationId == _conversation.ConversationId && message.FromEmail != _email)
            {
                AnsiConsole.Cursor.MoveLeft(Console.CursorLeft);
                AnsiConsole.Cursor.MoveDown();
                ConsoleWriteConversationMessage(message.Message, message.FromEmail, message.CreatedAt.ToLocalTime());
                AnsiConsole.WriteLine();
                await _conversationClient.MessageViewedAsync(new MessageViewedCommand { MessageId = message.MessageId });
            }
        }

        private static void ConsoleWriteMyMessage(string message, string fromEmail, DateTimeOffset createdAt)
        {
            AnsiConsole.Write(new Markup($"[grey][[{createdAt:HH:mm}]][/]]] [green]{fromEmail}[/]> {message}").LeftJustified());
            AnsiConsole.WriteLine();
        }

        private static void ConsoleWriteConversationMessage(string message, string fromEmail, DateTimeOffset createdAt)
        {
            AnsiConsole.Write(new Markup($"{message} <[blueviolet]{fromEmail}[/] [grey][[{createdAt:HH:mm}]][/]]]").RightJustified());
            AnsiConsole.WriteLine();
        }
    }

    public record ChatMessage(string Email, string Message, DateTimeOffset SentTime, bool Received = false);
}
