using System;
using System.Windows.Forms;
using Serilog;
using Serilog.Context;

namespace SeqWinFormApp
{
    /// <summary>
    /// 多專案日誌系統
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // 配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LogViewerForm());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "應用程式發生致命錯誤");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    /// <summary>
    /// 專案級別日誌配置
    /// </summary>
    public static class ProjectLogger
    {
        public static void LogWithProject(string projectName, string serviceName, string message, string level = "Information")
        {
            using (LogContext.PushProperty("Project", projectName))
            using (LogContext.PushProperty("Service", serviceName))
            {
                switch (level)
                {
                    case "Warning":
                        Log.Warning(message);
                        break;
                    case "Error":
                        Log.Error(message);
                        break;
                    default:
                        Log.Information(message);
                        break;
                }
            }
        }
    }
}
