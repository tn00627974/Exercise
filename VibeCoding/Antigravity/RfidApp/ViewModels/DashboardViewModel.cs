using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RfidApp.Models;
using RfidApp.Services;

namespace RfidApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _totalInventory;
        private int _activeRepairs;
        private int _lowStockItems;

        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
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
            IsBusy = true;
            
            var inventory = await _dataService.GetInventoryItemsAsync();
            var repairs = await _dataService.GetRepairTicketsAsync();

            TotalInventory = inventory.Sum(i => i.Quantity);
            LowStockItems = inventory.Count(i => i.Quantity < 5);
            ActiveRepairs = repairs.Count(r => r.Status != RepairStatus.Closed && r.Status != RepairStatus.Completed);

            IsBusy = false;
        }
    }
}
