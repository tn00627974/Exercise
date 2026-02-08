using MedicalAssistiveDevices.Hubs;
using MedicalAssistiveDevices.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服務
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MedicalDevicePolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://localhost:5001")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddHostedService<MedicalDeviceSimulator>();

var app = builder.Build();

app.UseCors("MedicalDevicePolicy");
app.UseRouting();

app.MapHub<MedicalDeviceHub>("/medicalhub");
app.MapControllers();

app.Run();