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
            await _viewModel.LoadTicketsCommand.ExecuteAsync();
        }
    }
}
