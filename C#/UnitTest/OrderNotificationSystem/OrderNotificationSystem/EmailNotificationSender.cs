


namespace OrderNotificationSystem
{
    public class EmailNotificationSender : INotificationSender
    {
        public NotificationResult Send(Order order)
        {
            return new NotificationResult
            {
                Success = true,
                Message = $"Email Sent to CustomEmail"
            };
        }
    }
}
