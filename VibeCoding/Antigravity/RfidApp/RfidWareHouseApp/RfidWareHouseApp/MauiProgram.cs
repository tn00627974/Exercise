using Microsoft.Extensions.Logging;
using RfidWareHouseApp.Services;
using RfidWareHouseApp.ViewModels;
using RfidWareHouseApp.Views;


namespace RfidWareHouseApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //MauiProgram.CreateMauiApp();
            var builder = MauiApp.CreateBuilder();

            // 註冊 Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IDataService, MockDataService>();

            // 註冊 ViewModels
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RepairViewModel>();
            builder.Services.AddTransient<InventoryViewModel>();

            // 註冊 Pages
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<RepairPage>();
            builder.Services.AddSingleton<InventoryPage>();
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

            return builder.Build();
        }
    }
}
