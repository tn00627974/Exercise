using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderNotificationSystem.Decorators
{
    public class LoggingNotificationSenderDecorator : INotificationSender
    {
        private readonly INotificationSender _innerSender;

        public LoggingNotificationSenderDecorator(INotificationSender innerSender)
        {
            _innerSender = innerSender ?? throw new ArgumentNullException(nameof(innerSender));
        }

        public NotificationResult Send(Order order)
        {
            var senderType = _innerSender.GetType().Name;
            Console.WriteLine($"[LOG] 開始發送通知 - 發送器: {senderType}, 訂單: {order.OrderNo}");


            try
            {
                var result = _innerSender.Send(order);
                var status = result.Success ? "成功" : "失敗";

                Console.WriteLine($"[LOG] 發送{status} - 發送器: {senderType}, 訊息: {result.Message}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOG] 發送異常 - 發送器: {senderType}, 異常: {ex.Message}");
                throw;
            }
        }
    }
}
