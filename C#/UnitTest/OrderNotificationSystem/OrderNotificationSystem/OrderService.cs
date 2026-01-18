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
        private readonly IOrderValidator _validator;

        // 👉 DI：從外面注入
        public OrderService(IOrderValidator validator, INotificationSender sender )
        {
            _sender = sender;
            _validator = validator;
        }

        public NotificationResult PlaceOrder(Order order)
        {
            // 第一步 : 驗證訂單
            var vaildationResult = _validator.Validate(order);
            if (!vaildationResult.Success) // 驗證失敗
            {
                return vaildationResult; // 返回錯誤訊息
            }

            // 第二步：驗證通過，發送通知
            _sender.Send(order);
            return new NotificationResult
            {
                Success = true,
                Message = $"{order.OrderNo}:金額為{order.Amount}元"
            };                      
        }
    }
}