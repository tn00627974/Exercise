using MongoDB.Bson;
using MongoDB.Driver;
using MongoLa.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddSingleton<ReadingService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/readings/{deviceId}", async (string deviceId, ReadingService svc) =>
    await svc.GetLatestAsync(deviceId));

app.MapGet("/readings1/{deviceId}", async (string deviceId, ReadingService svc) =>
    await svc.GetLatestAsync1(deviceId));

//builder.Services.AddSingleton<Reading>(); Reading 是資料模型（一筆發電讀數），不是服務。註冊它等於跟 DI 說「整個應用程式共用同一筆讀數」——沒有意義。
app.MapGet("/readings/{deviceId}/hourly", async (string deviceId, ReadingService svc) =>
    await svc.GetHourlyAverageAsync(deviceId));

app.Run();


