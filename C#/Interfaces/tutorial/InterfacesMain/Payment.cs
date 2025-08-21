using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace InterfacesMain
{
    public class Payment
    {
        public interface IPaymentProcessor 
        {
            void ProcessPayment(decimal amount);
        }

        public class CreditCardProcessor : IPaymentProcessor
        {
            public void ProcessPayment(decimal amount) { Console.WriteLine($"Processing credit card payment if {amount}"); }
        }

        public class PaypalProcessor : IPaymentProcessor
        {
            public void ProcessPayment(decimal amount) { Console.WriteLine($"Processing paypal payment if {amount}"); }
        }

        public class PaymentService
        {
            private readonly IPaymentProcessor _processor;

            public PaymentService(IPaymentProcessor processor)
            {
                _processor = processor; 
            }

            public void ProcessOrderPayment(decimal amount)
            {
                _processor.ProcessPayment(amount);
            }
        }
    }
}
