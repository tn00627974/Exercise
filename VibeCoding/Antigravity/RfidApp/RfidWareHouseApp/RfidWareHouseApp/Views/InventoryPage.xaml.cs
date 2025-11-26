using RfidApp.ViewModels;

namespace RfidApp.Views
{
    public partial class InventoryPage : ContentPage
    {
        InventoryViewModel _viewModel;

        public InventoryPage(InventoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // 修正：Command 類型沒有 ExecuteAsync 方法，應直接呼叫 Execute(null)
            _viewModel.LoadItemsCommand.Execute(null);
            // 如果 LoadItemsCommand 是非同步操作，請在 ViewModel 內處理非同步邏輯
        }
    }
}
