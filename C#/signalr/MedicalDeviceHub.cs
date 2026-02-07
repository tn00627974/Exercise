using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MedicalAssistiveDevices.Hubs
{
    /// <summary>
    /// 醫療輔具 SignalR Hub - 處理醫療設備與客戶端的即時通訊
    /// </summary>
    public class MedicalDeviceHub : Hub
    {
        private readonly ILogger<MedicalDeviceHub> _logger;

        public MedicalDeviceHub(ILogger<MedicalDeviceHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 客戶端連接時觸發
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"客戶端已連接: {connectionId}");
            
            await Clients.Caller.SendAsync("Connected", new
            {
                ConnectionId = connectionId,
                Timestamp = DateTime.UtcNow,
                Message = "成功連接到醫療設備監控系統"
            });

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 客戶端斷開連接時觸發
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"客戶端已斷開: {connectionId}");
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 訂閱特定醫療設備
        /// </summary>
        public async Task SubscribeToDevice(string deviceId, string patientId)
        {
            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            _logger.LogInformation($"客戶端 {Context.ConnectionId} 訂閱設備 {deviceId}，患者 {patientId}");
            
            await Clients.Caller.SendAsync("SubscriptionConfirmed", new
            {
                DeviceId = deviceId,
                PatientId = patientId,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 取消訂閱醫療設備
        /// </summary>
        public async Task UnsubscribeFromDevice(string deviceId, string patientId)
        {
            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            _logger.LogInformation($"客戶端 {Context.ConnectionId} 取消訂閱設備 {deviceId}");
            
            await Clients.Caller.SendAsync("UnsubscriptionConfirmed", new
            {
                DeviceId = deviceId,
                PatientId = patientId,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 發送醫療設備數據（由設備端調用）
        /// </summary>
        public async Task SendDeviceData(MedicalDeviceData data)
        {
            var groupName = $"Device_{data.DeviceId}_Patient_{data.PatientId}";
            
            // 檢查數據是否異常
            if (data.IsAbnormal)
            {
                await Clients.Group(groupName).SendAsync("AbnormalDataAlert", data);
                _logger.LogWarning($"異常數據警報: {data.DeviceId} - {data.MeasurementType}: {data.Value}");
            }
            
            // 發送數據到訂閱的客戶端
            await Clients.Group(groupName).SendAsync("ReceiveDeviceData", data);
            
            _logger.LogInformation($"設備數據已發送: {data.DeviceId} - {data.MeasurementType}");
        }

        /// <summary>
        /// 發送設備狀態更新
        /// </summary>
        public async Task UpdateDeviceStatus(string deviceId, string status, string message)
        {
            await Clients.All.SendAsync("DeviceStatusUpdated", new
            {
                DeviceId = deviceId,
                Status = status,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 請求歷史數據
        /// </summary>
        public async Task RequestHistoricalData(string deviceId, string patientId, DateTime startTime, DateTime endTime)
        {
            // 這裡應該從數據庫查詢歷史數據
            _logger.LogInformation($"歷史數據請求: {deviceId}, {patientId}, {startTime} - {endTime}");
            
            // 模擬返回歷史數據
            await Clients.Caller.SendAsync("HistoricalDataResponse", new
            {
                DeviceId = deviceId,
                PatientId = patientId,
                StartTime = startTime,
                EndTime = endTime,
                DataCount = 0,
                Message = "歷史數據查詢完成"
            });
        }
    }

    /// <summary>
    /// 醫療設備數據模型
    /// </summary>
    public class MedicalDeviceData
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string PatientId { get; set; }
        public string MeasurementType { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAbnormal { get; set; }
        public string Notes { get; set; }
    }
}