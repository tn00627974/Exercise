    namespace OrderNotificationSystem
{
    public class FakeNotificationSender : INotificationSender
    {
        private readonly bool _shouldSucceed;   
        public FakeNotificationSender(bool shouldSucceed) 
        {
            _shouldSucceed = shouldSucceed;    
        }
        public NotificationResult Send(Order order)
        {
            return new NotificationResult
            {
                Success = _shouldSucceed,
                Message = _shouldSucceed
                ? $"{order.OrderNo}:Notification sent successfully." 
                : "Failed to send notification."
            };
        }
    }
}
