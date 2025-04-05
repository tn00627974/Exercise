//using HelloWorld.Models;
using HelloWorld.MysqlModels; // 引用 MySQL Models
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

 //註冊 WebContext 到 DI 容器
builder.Services.AddDbContext<WebContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
options.UseMySQL(builder.Configuration.GetConnectionString("MySQLConnection")));

Console.WriteLine("MySQL 連線測試中...");
var app = builder.Build();

// 可以測試MySQL連線
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WebContext>();
    try
    {
        dbContext.Database.OpenConnection();
        Console.WriteLine("MySQL 連線成功！");
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"MySQL 連線失敗: {ex.Message}");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
