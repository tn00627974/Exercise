using System;
using Serilog;
using Serilog.Context;

namespace SimpleLogger
{
    /// <summary>
    /// 簡單的專案日誌類 - 可直接引用使用
    /// </summary>
    public class ProjectLogger
    {
        private static readonly string _seqUrl = "http://localhost:5341";
        private readonly ILogger _logger;
        private readonly string _projectName;

        /// <summary>
        /// 初始化日誌器，指定專案名稱
        /// </summary>
        public ProjectLogger(string appName)
        {
            _projectName = projectName;
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Application", appName)
                .Enrich.FromLogContext()
                .WriteTo.Seq(_seqUrl)
                .WriteTo.File($"logs/{appName}-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        /// <summary>
        /// 記錄資訊日誌
        /// </summary>
        public void Info(string message, string service = "", string module = "")
        {
            using (LogContext.PushProperty("Service", service))
            using (LogContext.PushProperty("Module", module))
            {
                _logger.Information(message);
            }
        }

        /// <summary>
        /// 記錄警告日誌
        /// </summary>
        public void Warning(string message, string service = "", string module = "")
        {
            using (LogContext.PushProperty("Service", service))
            using (LogContext.PushProperty("Module", module))
            {
                _logger.Warning(message);
            }
        }

        /// <summary>
        /// 記錄錯誤日誌
        /// </summary>
        public void Error(Exception ex, string message, string service = "", string module = "")
        {
            using (LogContext.PushProperty("Service", service))
            using (LogContext.PushProperty("Module", module))
            {
                _logger.Error(ex, message);
            }
        }

        public void Debug(string message, string service = "", string module = "")
        {
            using (LogContext.PushProperty("Service", service))
            using (LogContext.PushProperty("Module", module))
            {
                _logger.Debug(message);
            }
        }
    }
}
