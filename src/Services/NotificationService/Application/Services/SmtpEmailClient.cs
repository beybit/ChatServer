using NotificationService.Application.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace NotificationService.Application.Services
{
    public class SmtpEmailClient(IConfiguration configuration, ILogger<SmtpEmailClient> logger) : IEmailNotificationClient
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient
            {
                Host = configuration.GetValue("Smtp:Host", "smtp.gmail.com")!,
                Port = configuration.GetValue("Smtp:Port", 587),
                EnableSsl = configuration.GetValue("Smtp:EnableSsl", true),
                Credentials = new NetworkCredential(configuration.GetValue<string>("Smtp:Username"), configuration.GetValue<string>("Smtp:Password")),
            })
            {
                try
                {

                    MailMessage email = new MailMessage
                    {
                        From = new MailAddress(configuration.GetValue("Smtp:From", "noreply@online.chat")!),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    email.To.Add(to);

                    client.SendAsync(email, null);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Could not send email to: {To}", to);
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
    }
}
