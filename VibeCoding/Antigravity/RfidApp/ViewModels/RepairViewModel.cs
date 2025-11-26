using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RfidApp.Models;
using RfidApp.Services;

namespace RfidApp.ViewModels
{
    public class RepairViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        public ObservableCollection<RepairTicket> Tickets { get; } = new();

        public RepairViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadTicketsCommand = new Command(async () => await LoadTickets());
            NewTicketCommand = new Command(async () => await CreateTicket());
        }

        public Command LoadTicketsCommand { get; }
        public Command NewTicketCommand { get; }

        private async Task LoadTickets()
        {
            IsBusy = true;
            Tickets.Clear();
            var tickets = await _dataService.GetRepairTicketsAsync();
            foreach (var ticket in tickets)
            {
                Tickets.Add(ticket);
            }
            IsBusy = false;
        }

        private async Task CreateTicket()
        {
            // In a real app, navigate to a "New Ticket" page
            await Shell.Current.DisplayAlert("Not Implemented", "Navigate to New Ticket Page", "OK");
        }
    }
}
