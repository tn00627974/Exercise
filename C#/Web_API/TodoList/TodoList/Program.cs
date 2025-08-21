using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Todo.Models;
using Todo.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper 所有Profile
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
// AutoMapper 單一Profile
builder.Services.AddAutoMapper(typeof(TodoProfile)); 


// Add DbContext 
builder.Services.AddDbContext<TodoContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

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
