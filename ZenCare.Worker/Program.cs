using ZenCare.Worker;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
