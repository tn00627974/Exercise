//using SeqMultiService;
//using Serilog;
//using System;
//using System.Windows.Forms;

//namespace SeqWinFormApp
//{
//    class Program
//    {
//        [STAThread]
//        static void Main(string[] args)
//        {
//            Console.WriteLine("=== 多服務日誌演示 ===\n");

//            // 1. 用戶服務
//            Console.WriteLine("▶ 用戶服務操作...");
//            var userService = new UserService("UserService");
//            userService.CreateUser("john_doe", "john@example.com");
//            userService.LoginUser("john_doe");
//            Console.WriteLine();

//            // 2. 訂單服務
//            Console.WriteLine("▶ 訂單服務操作...");
//            var orderService = new OrderService("OrderService");
//            orderService.CreateOrder("user-123", 99.99m);
//            orderService.ProcessPayment("order-456", 99.99m);
//            Console.WriteLine();

//            // 3. 支付服務
//            Console.WriteLine("▶ 支付服務操作...");
//            var paymentService = new PaymentService("PaymentService");
//            paymentService.ProcessRefund("order-456", 99.99m);
//            Console.WriteLine();

//            // 4. 測試錯誤場景
//            Console.WriteLine("▶ 測試錯誤場景...");
//            try
//            {
//                orderService.CreateOrder("user-123", -10);
//            }
//            catch { }

//            Console.WriteLine("\n✅ 所有日誌已發送到 Seq Server (http://localhost:5341)");
//            Console.WriteLine("📁 本地日誌已保存到 logs/ 目錄");
//            MessageBox.Show("完成");
//        }
//    }
//}
