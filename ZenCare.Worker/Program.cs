using ZenCare.Worker;
using Microsoft.EntityFrameworkCore;
using ZenCare.Services;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Services;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<ZenCareDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
