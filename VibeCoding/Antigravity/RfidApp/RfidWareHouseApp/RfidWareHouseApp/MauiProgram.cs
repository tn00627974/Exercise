using Microsoft.Extensions.Logging;
using RfidApp.Services;
using RfidApp.ViewModels;


namespace RfidWareHouseApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //MauiProgram.CreateMauiApp();
            var builder = MauiApp.CreateBuilder();

            //builder.Services.AddSingleton<IDataService>();
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RepairViewModel>();
            builder.Services.AddTransient<InventoryViewModel>();

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
