using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp;

public class ConsoleBackgroundService : BackgroundService
{
    private readonly IConfiguration configuration;

    public ConsoleBackgroundService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
            Console.WriteLine(this.configuration.GetValue<int>("ConfigurationManagement:IsBasketEnabled"));
        }
    }
}