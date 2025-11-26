using Microsoft.Extensions.Logging;
using RfidApp.Services;
using RfidApp.ViewModels;
using RfidApp.Views;

namespace RfidApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IDataService, MockDataService>();
            builder.Services.AddSingleton<IRfidService, RfidService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<InventoryViewModel>();
            builder.Services.AddTransient<RepairViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<InventoryPage>();
            builder.Services.AddTransient<RepairPage>();

            return builder.Build();
        }
    }
}
