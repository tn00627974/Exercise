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
            await _viewModel.LoadItemsCommand.ExecuteAsync();
        }
    }
}
