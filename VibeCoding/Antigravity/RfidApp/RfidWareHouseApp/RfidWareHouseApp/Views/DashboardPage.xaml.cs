using RfidWareHouseApp.ViewModels;

namespace RfidWareHouseApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        DashboardViewModel _viewModel;

        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadStats();
        }
    }
}
