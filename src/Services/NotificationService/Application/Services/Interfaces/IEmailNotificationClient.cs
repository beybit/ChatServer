namespace NotificationService.Application.Services.Interfaces
{
    public interface IEmailNotificationClient
    {
        Task SendAsync(string to, string subject, string body);
    }
}
