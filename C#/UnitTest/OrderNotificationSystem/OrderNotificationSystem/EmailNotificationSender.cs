namespace OrderNotificationSystem
{
    public class EmailNotificationSender : INotificationSender
    {
        public NotificationResult Send(Order order)
        {
            var result = new NotificationResult
            {
                Success = true,
                Message = $"Email Sent to CustomEmail"
            };
            Console.WriteLine($"{result.Message}:{result.Success}");
            return result;
        }
    }
}
