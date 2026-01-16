using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderNotificationSystem
{
    public class OrderService
    {
        private readonly INotificationSender _sender;

        // 👉 DI：從外面注入
        public OrderService(INotificationSender sender)
        {
            _sender = sender;
        }

        public NotificationResult PlaceOrder(Order order)
        {
            if (order.Amount <= 0)
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = "訂單金額錯誤"
                };
            }

            return _sender.Send(order);
        }
    }
}
