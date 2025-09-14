namespace DelegateExample
{
    internal class Delegate1
    {
        #region
        public delegate void MyDelegate(); // 宣告委派
        public MyDelegate _myDelegate; // 宣告委派變數

        public delegate int MythDelegate(int a , int b); // 宣告委派
        public MythDelegate _mythDelegate; // 宣告委派變數
        #endregion

        #region Action 用於沒有回傳值的方法 
        public Action<int> _myAction1; // 用Action同時宣告 及 宣告委派變數
        public Action<string> _myAction2; // 用Action同時宣告 及 宣告委派變數
        #endregion

        #region Func 用於有回傳值的方法 
        public Func<int, int> _myAction3; // 用Func同時宣告 及 宣告委派變數 | <int,int> 第一個int是參數，第二個int是回傳值
        public Func<string,string> _myAction4; // 用Func同時宣告 及 宣告委派變數 | <string,string> 第一個string是參數，第二個string是回傳值
        #endregion

        #region Lambda 表達式
        public Action _myAction5;
        public Func<int, int, int> _mythDelegate1;
        #endregion

        public void Main()
        {
            // 多重委派
            _myDelegate += SendMsg0;
            _myDelegate += PrintSum;
            _myDelegate();

            // 移除方法
            _myDelegate -= SendMsg0;
            _myDelegate -= PrintSum;
            //_myDelegate(); // **因為移除了方法，如果是空的這一行會報錯

            // 判斷委派為空的寫法
            if (_myDelegate != null) { _myDelegate(); } // 判斷是不是為空
            _myDelegate?.Invoke(); // 為空更簡潔的寫法

            // 帶入Action <int> 
            _myAction1 += SendMsg1;
            _myAction1?.Invoke(5); // 為空更簡潔的寫法


            // 帶入Action <string>
            _myAction2 += SendMsg2;
            _myAction2?.Invoke("我是XXX"); // 為空更簡潔的寫法

            // 帶入Func <int,int>
            _myAction3 += SendMsg3;
            _myAction3?.Invoke(10); // 為空更簡潔的寫法

            // 帶入Func <string , string >
            _myAction4 += SendMsg4;
            _myAction4?.Invoke("我是OOO"); // 為空更簡潔的寫法

            // Lambda 表達式
            _myAction5 = () =>
            {
                Console.WriteLine("Hello World Lambda");
            };

            _myAction5 = () => Console.WriteLine("Hello World Lambda 簡寫"); // 一行寫法

            _mythDelegate = AddNumner;
            int result = _mythDelegate(3, 5);
            Console.WriteLine($"_mythDelegate : {result}");

            _mythDelegate1 = (a,b) => a + b; // Lambda + Func 一行寫法簡寫(有回傳值) | 不用多寫 AddNumner 方法
            int result2 = _mythDelegate1(7, 8);
            Console.WriteLine($"_mythDelegate1 : {result2}");


        }

        private void SendMsg0()
        {
            Console.WriteLine($"Hello World");
        }


        private void SendMsg1(int number)
        {
            Console.WriteLine($"Hello World，這是數字{number}");
        }

        private void SendMsg2(string msg)
        {
            Console.WriteLine("Hello World2"+msg);
        }

        private int SendMsg3(int number)
        {
            return number;
        }

        private string SendMsg4(string msg)
        {
            return "Hello World2" + msg;
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

        private int AddNumner(int a , int b)
        {
            return a + b;
        }
    }
}
