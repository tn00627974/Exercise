using DelegateExample6;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace DelegateExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Delegate1 dg = new Delegate1();
            //dg.Main();

            //Delegate2 dg2 = new Delegate2();
            //dg2.Main();

            //Delegate3 dg3 = new Delegate3();
            //dg3.Main();

            //Delegate4 gameManager = new Delegate4();
            //Delegate4 gameManager = new Delegate4();
            //gameManager.Main();

            //Delegate5 dg5 = new Delegate5();
            //dg5.Main();

            Delegate6 dg6 = new Delegate6();
            dg6.Main();
        }
    }
}