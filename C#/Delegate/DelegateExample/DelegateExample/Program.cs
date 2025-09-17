using System;
using static System.Net.Mime.MediaTypeNames;

namespace DelegateExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Delegate1 dg = new Delegate1();
            //dg.Main();
            dg.Test();

            //Delegate2 dg2 = new Delegate2();
            //dg2.Main();

            //Delegate3 dg3 = new Delegate3();
            //dg3.Main();

            //Delegate4 gameManager = new Delegate4();
            //gameManager.Main();
        }
    }
}