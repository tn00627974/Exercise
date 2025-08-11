using System;
using static InterfacesMain.Animal;
using static InterfacesMain.Cars;

namespace InterfacesMain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Cat mini = new Cat();
            mini.Eat("fish");
            mini.MakeSound();

            Dog chocho = new Dog();
            chocho.Eat("cesar");
            chocho.MakeSound();

            Toyota rav4 = new Toyota();
            rav4.Drive("Taipei");

            Toyota fit = new Toyota();
            rav4.Drive("Tainan");
        }


    }
}
