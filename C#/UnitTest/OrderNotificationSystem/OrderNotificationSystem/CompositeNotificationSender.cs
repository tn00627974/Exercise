using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderNotificationSystem
{
    public class CompositeNotificationSender : INotificationSender
    {
        private readonly IEnumerable<INotificationSender> _senders;
        public CompositeNotificationSender(params INotificationSender[] senders)
        {
            _senders = senders ?? Array.Empty<INotificationSender>();
        }

        public NotificationResult Send(Order order)
        {
            // 檢查是否有配置發送器
            if (!_senders.Any())
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = "沒有配置任何通知發送器"
                };
            }

            var results = new List<NotificationResult>();
            var failedMessages = new List<string>();

            foreach (var sender in _senders)
            {
                try 
                {
                    var orderResult = sender.Send(order);
                    results.Add(orderResult);
                    if (!orderResult.Success)
                    {
                        failedMessages.Add(orderResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    failedMessages.Add($"發送失敗: {ex.Message}");
                }
            }

            // 決定整體結果
            if (failedMessages.Any())
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = $"部分通知失敗: {string.Join("; ", failedMessages)}"
                };
            }

            return new NotificationResult
            {
                Success = true,
                Message = $"所有通知已發送 ({results.Count} 個)"
            };
        }
    }
}
