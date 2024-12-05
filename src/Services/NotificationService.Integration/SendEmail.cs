namespace NotificationService.Integration
{
    public class SendEmail
    {
        public Guid MessageId { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
