namespace OrderNotificationSystem
{
    public class LineNotificationSender : INotificationSender
    {
        public NotificationResult Send(Order order)
        {
            var result = new NotificationResult
            {
                Success = true,
                Message = $"Line Sent to {order.OrderNo}"
            };
            Console.WriteLine($"{result.Message}:{result.Success}");
            return result;
        }

    }
}
