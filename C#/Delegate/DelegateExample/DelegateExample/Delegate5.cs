using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static DelegateExample.Delegate5;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DelegateExample
{
    public class Delegate5
    {
        private delegate void Calculate(int a, int b);
        private Calculate _calculate;
        //public delegate void CalculateFloat(float a, float b);
        //public CalculateFloat calculateFloat;

        // 🎯 進階挑戰 Action , Func
        private Action<int, int> _action;
        private Func<int, int, int> _func;
        private Func<int, int, float> _funcFloat;


        public void Main()
        {
            int a = 3;
            int b = 5;
            //_calculate = Add;
            //_calculate(a, b);
            //_calculate = Subtract;
            //_calculate(a, b);
            //_calculate = Multiply;
            //_calculate(a, b);
            //_calculate = Divide;
            //_calculate(a, b);

            //// 改用 Action <int, int> 來處理「印出結果」。
            //_action = Add;
            //_action(a, b);
            //_action = Subtract;
            //_action(a, b);
            //_action = Multiply;
            //_action(a, b);
            //_action = Divide;
            //_action(a, b);

            // 改用 Func <int, int, int> 來處理「運算邏輯」。
            _func = Add;
            _func(a,b);
            _func = Subtract;
            _func(a, b);
            _func = Multiply;
            _func(a, b);
            _funcFloat = Divide;
            _funcFloat(a, b);
        }

        #region Lambda 表達式
        // 無回傳值的 Lambda
        //static void ((int a, int b) => Console.WriteLine($"加法: {a + b}");
        //static void Subtract(int a, int b) => Console.WriteLine($"減法: {a - b}");
        //static void Multiply(int a, int b) => Console.WriteLine($"乘法: {a * b}");
        //static void Divide(float a, float b) => Console.WriteLine($"除法: {a / b}"); // float 版本
        //static void Divide(int a, int b) => Console.WriteLine($"除法: {(float)a / b}");

        // 有回傳值的 Lambda
        static int Add(int a, int b) => Log("加法",a + b);
        static int Subtract(int a, int b) => Log("減法", a - b);
        static int Multiply(int a, int b) => Log("乘法", a * b);
        static float Divide(int a, int b) => Log("除法", (float)a / b);

        public static int Log(string msg, int sum)
        {
            Console.WriteLine($"{msg}:{sum}");
            return sum;
        }

        public static float Log(string msg, float sum)
        {
            Console.WriteLine($"{msg}:{sum}");
            return (int)sum;
        }


        #endregion

        #region 一般方法
        //public void Add(int a, int b)
        //{
        //    Console.WriteLine($"加法: {a + b}");
        //}
        //public void Subtract(int a, int b)
        //{
        //    Console.WriteLine($"減法: {a - b}");
        //}
        //public void Multiply(int a, int b)
        //{
        //    Console.WriteLine($"乘法: {a + b}");
        //}
        //public void Divide(int a, int b)
        //{
        //    Console.WriteLine($"除法: {(float)a / b}");
        //}
        #endregion

        #region 一般方法(有回傳值)
        //public int Add(int a, int b)
        //{
        //    int sum = a + b;
        //    Console.WriteLine($"加法: {sum}");
        //    return sum;
        //}
        //public int Subtract(int a, int b)
        //{
        //    int sum = a - b;
        //    Console.WriteLine($"減法: {sum}");
        //    return sum;
        //}
        //public int Multiply(int a, int b)
        //{
        //    int sum = a * b;
        //    Console.WriteLine($"乘法: {sum}");
        //    return sum;
        //}
        //public int Divide(int a, int b)
        //{
        //    int sum = a / b;
        //    Console.WriteLine($"除法: {sum}");
        //    return sum;
        //}

        #endregion


    }



}
    