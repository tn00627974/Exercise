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
            _senders = senders;
        }

        public NotificationResult Send(Order order)
        {
            var reuslts = new List<NotificationResult>();
            foreach (var sender in _senders)
            {
                var orderRusult = sender.Send(order);
                reuslts.Add(orderRusult);
                if (!orderRusult.Success)
                {
                    Console.WriteLine("Failed to send notification to " + sender.GetType().Name);
                }
            }
            Console.WriteLine("ok" );
            return new NotificationResult { Success = true, Message = "test" };
        }



        public void test()
        {
            var order = new Order { OrderNo = "ORD001", Amount = 1000 };

            var compostite = new CompositeNotificationSender(
               new EmailNotificationSender(),
                      new LineNotificationSender());

            compostite.Send(order);
        }
    }



}
