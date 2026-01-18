namespace OrderNotificationSystem
{
    public class DefaultOrderValidator : IOrderValidator
    {
        public NotificationResult Validate(Order order)
        {
            if (order.Amount <= 0)
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = "訂單錯誤"
                };
            }

            return new NotificationResult
            {
                Success = true,
                Message = "驗證通過"
            };
        }
    }
}
