using System;

namespace InterfacesMain
{
    public class Animal
    {
        public interface IAnimal
        {
            void Eat(string food); // eat method
            void MakeSound(); // some sound
        }

        public class Cat : IAnimal
        {
            public void Eat(string food) { Console.WriteLine("Cat is eating " + food); }
            public void MakeSound() { Console.WriteLine("Meow~ Meow~"); }
        }

        public class Dog : IAnimal
        {
            public void Eat(string food) { Console.WriteLine("Dog is eating " + food); }
            public void MakeSound() { Console.WriteLine("Woof~ Woof~"); }
        }
    }
}
