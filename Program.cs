using WindowsServiceDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() // Enables Windows Service integration
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>(); // Registers the Worker service
    })
    .Build();

await host.RunAsync();

//var host = builder.Build();
//host.Run();
