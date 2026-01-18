using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderNotificationSystem
{
    public interface IOrderValidator
    {
        public NotificationResult Validate(Order order);
    }
}
