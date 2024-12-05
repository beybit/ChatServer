using MassTransit;
using NotificationService.Application.Services.Interfaces;
using NotificationService.Integration;

namespace NotificationService.Application.Features.Email.Consumers
{
    public class SendEmailConsumer (ILogger<SendEmailConsumer> logger, IEmailNotificationClient emailNotificationClient) : IConsumer<SendEmail>
    {
        public async Task Consume(ConsumeContext<SendEmail> context)
        {
            logger.LogInformation("Send email: {@Email}", context.Message);
            await emailNotificationClient.SendAsync(context.Message.Email, context.Message.Subject, context.Message.Body);
        }
    }
}
