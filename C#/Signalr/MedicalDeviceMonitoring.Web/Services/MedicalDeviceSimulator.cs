using MedicalDeviceMonitoring.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MedicalDeviceMonitoring.Web.Services
{
    /// <summary>
    /// 醫療設備模擬器 - 模擬各種醫療設備發送數據
    /// </summary>
    public class MedicalDeviceSimulator : BackgroundService
    {
        private readonly IHubContext<MedicalDeviceHub> _hubContext;
        private readonly ILogger<MedicalDeviceSimulator> _logger;
        private readonly Random _random = new Random();

        public MedicalDeviceSimulator(
            IHubContext<MedicalDeviceHub> hubContext,
            ILogger<MedicalDeviceSimulator> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("醫療設備模擬器已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SimulateBloodPressureMonitor("BP001", "P001");
                    await SimulateHeartRateMonitor("HR001", "P001");
                    await SimulateOxygenMonitor("OX001", "P001");
                    await SimulateThermometer("TH001", "P001");

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "設備模擬器發生錯誤");
                }
            }

            _logger.LogInformation("醫療設備模擬器已停止");
        }

        private async Task SimulateBloodPressureMonitor(string deviceId, string patientId)
        {
            var systolic = _random.Next(90, 160);
            var diastolic = _random.Next(60, 100);
            var isAbnormal = systolic > 140 || systolic < 90 || diastolic > 90 || diastolic < 60;

            var data = new MedicalDeviceData
            {
                DeviceId = deviceId,
                DeviceType = "血壓計",
                PatientId = patientId,
                MeasurementType = "血壓",
                Value = systolic,
                Unit = $"{systolic}/{diastolic} mmHg",
                Timestamp = DateTime.UtcNow,
                IsAbnormal = isAbnormal,
                Notes = isAbnormal ? "血壓異常，請注意" : "血壓正常"
            };

            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveDeviceData", data);

            if (isAbnormal)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AbnormalDataAlert", data);
            }
        }

        private async Task SimulateHeartRateMonitor(string deviceId, string patientId)
        {
            var heartRate = _random.Next(50, 120);
            var isAbnormal = heartRate > 100 || heartRate < 60;

            var data = new MedicalDeviceData
            {
                DeviceId = deviceId,
                DeviceType = "心率監測器",
                PatientId = patientId,
                MeasurementType = "心率",
                Value = heartRate,
                Unit = "bpm",
                Timestamp = DateTime.UtcNow,
                IsAbnormal = isAbnormal,
                Notes = isAbnormal ? "心率異常" : "心率正常"
            };

            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveDeviceData", data);

            if (isAbnormal)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AbnormalDataAlert", data);
            }
        }

        private async Task SimulateOxygenMonitor(string deviceId, string patientId)
        {
            var oxygenLevel = _random.Next(85, 100);
            var isAbnormal = oxygenLevel < 95;

            var data = new MedicalDeviceData
            {
                DeviceId = deviceId,
                DeviceType = "血氧監測器",
                PatientId = patientId,
                MeasurementType = "血氧濃度",
                Value = oxygenLevel,
                Unit = "%",
                Timestamp = DateTime.UtcNow,
                IsAbnormal = isAbnormal,
                Notes = isAbnormal ? "血氧濃度偏低" : "血氧濃度正常"
            };

            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveDeviceData", data);

            if (isAbnormal)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AbnormalDataAlert", data);
            }
        }

        private async Task SimulateThermometer(string deviceId, string patientId)
        {
            var temperature = 36.0 + (_random.NextDouble() * 2.5);
            var isAbnormal = temperature > 37.5 || temperature < 36.0;

            var data = new MedicalDeviceData
            {
                DeviceId = deviceId,
                DeviceType = "體溫計",
                PatientId = patientId,
                MeasurementType = "體溫",
                Value = Math.Round(temperature, 1),
                Unit = "°C",
                Timestamp = DateTime.UtcNow,
                IsAbnormal = isAbnormal,
                Notes = isAbnormal ? "體溫異常" : "體溫正常"
            };

            var groupName = $"Device_{deviceId}_Patient_{patientId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveDeviceData", data);

            if (isAbnormal)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AbnormalDataAlert", data);
            }
        }
    }

    /// <summary>
    /// 醫療設備數據模型
    /// </summary>
    public class MedicalDeviceData
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string MeasurementType { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsAbnormal { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}