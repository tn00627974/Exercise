using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace InterfacesMain
{
    public class Engine
    {
        public void Start()
        {
            Console.WriteLine("Engine started");
        }
    }

    // 組合
    class Car1
    {
        private Engine engine = new Engine();

        public void StartCar()
        {
            engine.Start(); // Car contains an Engine and uses it
            Console.WriteLine("Car is ready to drive");
        }
    }

    // 繼承
    public class Car : Engine
    {
        //private Engine engine = new Engine();

        public void StartCar()
        {
            //engine.Start(); // Car contains an Engine and uses it
            Console.WriteLine("Car is ready to drive");
        }
    }


}
