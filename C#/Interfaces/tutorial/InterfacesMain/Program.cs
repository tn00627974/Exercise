using System;
using static InterfacesMain.Animal;
using static InterfacesMain.Cars;
using static InterfacesMain.Payment;

namespace InterfacesMain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Animal
            Cat mini = new Cat();
            mini.Eat("fish");
            mini.MakeSound();

            Dog chocho = new Dog();
            chocho.Eat("cesar");
            chocho.MakeSound();

            // Car
            Toyota rav4 = new Toyota();
            rav4.Drive("Taipei");

            Toyota fit = new Toyota();
            rav4.Drive("Tainan");

            // Payment 
            IPaymentProcessor creditCardProcessor = new CreditCardProcessor();
            creditCardProcessor.ProcessPayment(100.00m);

            IPaymentProcessor creditCardProcessor1 = new CreditCardProcessor();
            PaymentService paymentService = new PaymentService(creditCardProcessor1);
            paymentService.ProcessOrderPayment(200.00m);

        }


    }
}
