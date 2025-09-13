using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegateExample
{
    internal class Delegate1
    {
        public delegate void MyDelegate();

        private static MyDelegate _myDelegate;

        public void Main()
        {
            _myDelegate += SendMsg;
            _myDelegate += SendMsg2;
            _myDelegate += PrintSum;
            _myDelegate();
        }

        private void SendMsg()
        {
            Console.WriteLine("Hello World");
        }

        private void SendMsg2()
        {
            Console.WriteLine("Hello World2");
        }

        private void PrintSum()
        {
            int sum = 0;
            for (int i = 0; i <= 100; i++)
            {
                sum += i;
                Console.WriteLine($"數字:{i},總和:{sum}");
            }
        }        
    }
}
