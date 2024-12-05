using ChatService.Application.Features.Conversations.Consumers;
using ChatService.Domain;
using ChatService.Infrastructure;
using MassTransit;
using NotificationService.Integration;
using Quartz;

namespace ChatService.Application.Jobs
{
    public class SendUnviewedMessagesToEmailJob : IJob
    {
        readonly ILogger<SendUnviewedMessagesToEmailJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SendUnviewedMessagesToEmailJob(ILogger<SendUnviewedMessagesToEmailJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("Job {JobName} started", nameof(SendUnviewedMessagesToEmailJob));

            using (var scope = _serviceProvider.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var messageViewExpirationTime = DateTimeOffset.UtcNow.Add(-configuration.GetValue("MessageViewExpirationTime", TimeSpan.FromMinutes(1)));

                var viewExpiredMessages = from cm in dbContext.ConversationMessages
                                           where cm.Status == ConversationMessageStatus.Created && cm.CreatedAt < messageViewExpirationTime
                                           select new SendMessageEmail(cm.Id);

                await publishEndpoint.PublishBatch(viewExpiredMessages);
                await dbContext.SaveChangesAsync();
            }

            _logger.LogDebug("Job {JobName} completed", nameof(SendUnviewedMessagesToEmailJob));
        }
    }
}
