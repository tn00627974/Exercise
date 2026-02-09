using MedicalDeviceMonitoring.Web.Hubs;
using MedicalDeviceMonitoring.Web.Services;

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
        builder.WithOrigins("http://localhost:5278", "https://localhost:7028", "http://localhost:3000", "https://localhost:5001")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddHostedService<MedicalDeviceSimulator>();

var app = builder.Build();

app.UseCors("MedicalDevicePolicy");
app.UseDefaultFiles();      // ← 自動提供 index.html
app.UseStaticFiles();       // ← 返回文件
app.UseRouting();

app.MapHub<MedicalDeviceHub>("/medicalhub");
app.MapControllers();

app.Run();