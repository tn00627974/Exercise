using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RfidWareHouseApp.Models;
using RfidWareHouseApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RfidWareHouseApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _totalInventory;
        private int _activeRepairs;
        private int _lowStockItems;

        // 建構子供 DI 或程式碼直接注入 IDataService 使用
        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            LoadStatsCommand = new Command(async () => await LoadStats());
        }

        public int TotalInventory
        {
            get => _totalInventory; 
            set => SetProperty(ref _totalInventory, value);
        }

        public int ActiveRepairs
        {
            get => _activeRepairs;
            set => SetProperty(ref _activeRepairs, value);
        }

        public int LowStockItems
        {
            get => _lowStockItems;
            set => SetProperty(ref _lowStockItems, value);
        }

        public Command LoadStatsCommand { get; }

        public async Task LoadStats()
        {
            if (_dataService == null)
                return;

            IsBusy = true;

            var inventory = await _dataService.GetInventoryItemsAsync();
            var repairs = await _dataService.GetRepairTicketsAsync();

            TotalInventory = inventory?.Sum(i => i.Quantity) ?? 0;
            LowStockItems = inventory?.Count(i => i.Quantity < 5) ?? 0;
            ActiveRepairs = repairs?.Count(r => r.Status != RepairStatus.Closed && r.Status != RepairStatus.Completed) ?? 0;

            IsBusy = false;
        }
    }
}
