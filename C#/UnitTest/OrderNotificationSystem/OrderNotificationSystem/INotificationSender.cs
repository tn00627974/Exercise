using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OrderNotificationSystem.Models;

namespace OrderNotificationSystem
{
    public interface INotificationSender
    {
        public NotificationResult Send(Order order);
    }
}
