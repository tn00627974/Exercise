using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace InterfacesMain
{
    public class Cars
    {
        public interface ICars
        {
            void Drive(string direction);
        }

        public class Toyota : ICars
        {
            public void Drive(string direction) { Console.WriteLine($"Car is driving{direction}"); }
        }

        public class Honda : ICars
        {
            public void Drive(string direction) { Console.WriteLine($"Car is driving{direction}"); }
        }
    }
}
