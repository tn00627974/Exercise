using RfidApp.ViewModels;

namespace RfidApp.Views
{
    public partial class RepairPage : ContentPage
    {
        RepairViewModel _viewModel;

        public RepairPage(RepairViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // 修正：Command 類型沒有 ExecuteAsync 方法，應直接呼叫 ViewModel 的 LoadTickets 方法
            await _viewModel.LoadTickets();
        }
    }
}
