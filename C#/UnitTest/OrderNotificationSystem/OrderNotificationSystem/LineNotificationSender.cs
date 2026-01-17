namespace OrderNotificationSystem
{
    public class LineNotificationSender : INotificationSender
    {
        public NotificationResult Send(Order order)
        {
            return new NotificationResult
            {
                Success = true,
                Message = $"Line Sent to {order.OrderNo}"
            };
        }
    }
}
