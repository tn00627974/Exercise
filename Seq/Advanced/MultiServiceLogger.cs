using System;
using Serilog;
using Serilog.Context;

namespace SeqMultiService
{
    /// <summary>
    /// 日誌配置工廠 - 為每個服務創建獨立的日誌配置
    /// </summary>
    public static class LoggerFactory
    {
        public static ILogger CreateServiceLogger(string serviceName, string environment = "Development")
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                // 結構化日誌：添加服務識別屬性
                .Enrich.WithProperty("Service", serviceName)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithProperty("Version", "1.0.0")
                //.Enrich.WithMachineName()
                //.Enrich.WithThreadId()
                .Enrich.FromLogContext()
                // 發送到 Seq Server
                .WriteTo.Seq("http://localhost:5341")
                // 同時保存本地日誌
                .WriteTo.File(
                    $"logs/{serviceName}-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }

    /// <summary>
    /// 用戶服務 - 示例
    /// </summary>
    public class UserService
    {
        private readonly ILogger _logger;

        public UserService(string serviceName = "UserService")
        {
            _logger = LoggerFactory.CreateServiceLogger(serviceName);
        }

        public void CreateUser(string username, string email)
        {
            // 建立關聯 ID (用於追蹤整個操作)
            var correlationId = Guid.NewGuid().ToString();
            
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Module", "UserCreation"))
            using (LogContext.PushProperty("Action", "CreateUser"))
            {
                _logger.Information("開始建立用戶: {Username}, {Email}", username, email);

                try
                {
                    // 驗證
                    if (string.IsNullOrEmpty(username))
                    {
                        _logger.Warning("用戶名為空");
                        throw new ArgumentException("用戶名不能為空");
                    }

                    // 模擬業務邏輯
                    System.Threading.Thread.Sleep(500);

                    _logger.Information("用戶建立成功", new { Username = username, Email = email });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "建立用戶失敗: {Username}", username);
                    throw;
                }
            }
        }

        public void LoginUser(string username)
        {
            var correlationId = Guid.NewGuid().ToString();
            
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Module", "Authentication"))
            using (LogContext.PushProperty("Action", "Login"))
            {
                _logger.Information("用戶登入: {Username}", username);
                _logger.Information("登入成功");
            }
        }
    }

    /// <summary>
    /// 訂單服務 - 示例
    /// </summary>
    public class OrderService
    {
        private readonly ILogger _logger;

        public OrderService(string serviceName = "OrderService")
        {
            _logger = LoggerFactory.CreateServiceLogger(serviceName);
        }

        public void CreateOrder(string userId, decimal amount)
        {
            var correlationId = Guid.NewGuid().ToString();
            var orderId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Module", "OrderCreation"))
            using (LogContext.PushProperty("Action", "CreateOrder"))
            using (LogContext.PushProperty("OrderId", orderId))
            {
                _logger.Information("建立訂單: UserId={UserId}, Amount={Amount}", userId, amount);

                try
                {
                    if (amount <= 0)
                    {
                        _logger.Warning("無效金額: {Amount}", amount);
                        throw new ArgumentException("金額必須大於 0");
                    }

                    System.Threading.Thread.Sleep(800);

                    _logger.Information("訂單建立成功", new { OrderId = orderId, Amount = amount, UserId = userId });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "建立訂單失敗: UserId={UserId}", userId);
                    throw;
                }
            }
        }

        public void ProcessPayment(string orderId, decimal amount)
        {
            var correlationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Module", "Payment"))
            using (LogContext.PushProperty("Action", "ProcessPayment"))
            {
                _logger.Information("開始處理支付: OrderId={OrderId}, Amount={Amount}", orderId, amount);

                try
                {
                    System.Threading.Thread.Sleep(1000);
                    _logger.Information("支付成功");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "支付失敗: OrderId={OrderId}", orderId);
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// 支付服務 - 示例
    /// </summary>
    public class PaymentService
    {
        private readonly ILogger _logger;

        public PaymentService(string serviceName = "PaymentService")
        {
            _logger = LoggerFactory.CreateServiceLogger(serviceName);
        }

        public void ProcessRefund(string orderId, decimal amount)
        {
            var correlationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Module", "Refund"))
            using (LogContext.PushProperty("Action", "ProcessRefund"))
            {
                _logger.Information("開始處理退款: OrderId={OrderId}, Amount={Amount}", orderId, amount);

                try
                {
                    System.Threading.Thread.Sleep(600);
                    _logger.Information("退款成功");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "退款失敗");
                    throw;
                }
            }
        }
    }
}
