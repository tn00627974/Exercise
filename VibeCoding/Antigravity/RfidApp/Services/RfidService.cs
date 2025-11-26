using System;
using System.Threading.Tasks;
using System.Timers;

namespace RfidApp.Services
{
    public class RfidService : IRfidService
    {
        public event EventHandler<string> TagDetected;
        private Timer _timer;
        private bool _isScanning;
        private Random _random = new Random();

        public bool IsScanning => _isScanning;

        public RfidService()
        {
            _timer = new Timer(2000); // Simulate scan every 2 seconds
            _timer.Elapsed += OnTimerElapsed;
        }

        public Task StartScanningAsync()
        {
            if (!_isScanning)
            {
                _isScanning = true;
                _timer.Start();
            }
            return Task.CompletedTask;
        }

        public Task StopScanningAsync()
        {
            if (_isScanning)
            {
                _isScanning = false;
                _timer.Stop();
            }
            return Task.CompletedTask;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Simulate finding a tag
            // In a real app, this would come from the hardware SDK
            string[] mockTags = { "RFID-001", "RFID-002", "RFID-003", "RFID-999" };
            string detectedTag = mockTags[_random.Next(mockTags.Length)];
            
            TagDetected?.Invoke(this, detectedTag);
        }
    }
}
