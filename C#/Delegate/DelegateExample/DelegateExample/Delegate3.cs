using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DelegateExample
{
    internal class Delegate3
    {
        private Action _myAction;
        private Func<int,int> _myFunc;


        private async void DelayAction(int ms, Action callback)
        {
            await Task.Delay(ms);
            callback?.Invoke();
        }

        private async void DelayFunc(int ms, Func<int,int> callback)
        {
            await Task.Delay(ms);
            callback?.Invoke(10);
        }

        public void Main()
        {
            #region Action
            _myAction += SayHelloWorld;
            DelayAction(1000, _myAction);
            _myAction += SayGoodBye;
            DelayAction(2000, _myAction);
            #endregion

            #region Func
            _myFunc += PrintNumber;
            DelayFunc(3000, _myFunc);
            Console.ReadLine(); // 等使用者按 Enter，讓 async 有時間跑完
            #endregion

        }

        private void SayHelloWorld()
        {
            Console.WriteLine($"Hello World");
        }
        private void SayGoodBye()
        {
            Console.WriteLine($"Good Bye");
        }

        private int PrintNumber(int number)
        {
            Console.WriteLine($"數字為 : {number}");
            return number;
        }

    }
    
}
