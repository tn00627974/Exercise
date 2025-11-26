using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RfidApp.Models;
using RfidApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls; // for Application

namespace RfidApp.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IRfidService _rfidService;

        public ObservableCollection<InventoryItem> Items { get; } = new();

        // 公用無參數建構子：供 XAML (vm:InventoryViewModel) 使用，從 DI 容器解析依賴
        //public InventoryViewModel()
        //    : this(
        //          Application.Current?.Services?.GetService<IDataService>() ?? throw new InvalidOperationException("IDataService not registered"),
        //          Application.Current?.Services?.GetService<IRfidService>() ?? throw new InvalidOperationException("IRfidService not registered"))
        //{
        //}

        public InventoryViewModel(IDataService dataService, IRfidService rfidService)
        {
            _dataService = dataService;
            _rfidService = rfidService;

            LoadItemsCommand = new Command(async () => await LoadItems());
            StartScanCommand = new Command(async () => await StartScanning());
            
            _rfidService.TagDetected += OnTagDetected;
        }

        public Command LoadItemsCommand { get; }
        public Command StartScanCommand { get; }

        private async Task LoadItems()
        {
            IsBusy = true;
            Items.Clear();
            var items = await _dataService.GetInventoryItemsAsync();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            IsBusy = false;
        }

        private async Task StartScanning()
        {
            if (_rfidService.IsScanning)
                await _rfidService.StopScanningAsync();
            else
                await _rfidService.StartScanningAsync();
        }

        private void OnTagDetected(object sender, string tag)
        {
            // In a real app, we would filter the list or highlight the item
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var item = await _dataService.GetInventoryItemByRfidAsync(tag);
                if (item != null)
                {
                    await Shell.Current.DisplayAlert("Item Found", $"Scanned: {item.Name} ({item.Quantity})", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Unknown Tag", $"Tag: {tag}", "OK");
                }
            });
        }
    }
}
