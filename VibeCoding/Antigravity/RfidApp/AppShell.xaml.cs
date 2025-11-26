namespace RfidApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Register Routes
            Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            Routing.RegisterRoute(nameof(Views.DashboardPage), typeof(Views.DashboardPage));
            Routing.RegisterRoute(nameof(Views.InventoryPage), typeof(Views.InventoryPage));
            Routing.RegisterRoute(nameof(Views.RepairPage), typeof(Views.RepairPage));
        }
    }
}
