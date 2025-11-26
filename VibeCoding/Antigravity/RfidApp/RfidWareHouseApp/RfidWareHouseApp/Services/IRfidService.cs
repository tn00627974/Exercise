using System;
using System.Threading.Tasks;

namespace RfidWareHouseApp.Services
{
    public interface IRfidService
    {
        event EventHandler<string> TagDetected;
        Task StartScanningAsync();
        Task StopScanningAsync();
        bool IsScanning { get; }
    }
}
