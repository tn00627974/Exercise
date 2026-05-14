//using System;
//using System.Windows.Forms;
//using Serilog;

//namespace SeqWinFormApp
//{
//    static class Program
//    {
//        [STAThread]
//        static void Main()
//        {
//            // 配置 Serilog 連接到 Seq Server
//            Log.Logger = new LoggerConfiguration()
//                .MinimumLevel.Debug()
//                .WriteTo.Seq("http://localhost:5341") // Seq Server 地址
//                .CreateLogger();

//            try
//            {
//                Log.Information("應用程式啟動");
                
//                Application.EnableVisualStyles();
//                Application.SetCompatibleTextRenderingDefault(false);
//                Application.Run(new MainForm());
//            }
//            catch (Exception ex)
//            {
//                Log.Fatal(ex, "應用程式發生致命錯誤");
//            }
//            finally
//            {
//                Log.CloseAndFlush();
//            }
//        }
//    }
//}
