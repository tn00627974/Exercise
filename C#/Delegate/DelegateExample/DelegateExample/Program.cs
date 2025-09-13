using System;

namespace DelegateExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Delegate1 dg = new Delegate1();
            dg.Main();

            Delegate2 dg2 = new Delegate2();
            dg2.Main();

        }
    }
}