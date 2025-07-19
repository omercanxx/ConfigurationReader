using ConfigurationReader;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureConfigurationProvider()
            .ConfigureServices((hostContext, services) =>
            {
                var serviceConfigurator = new ServiceConfigurator(hostContext.Configuration);
                serviceConfigurator.ConfigureServices(services);
            });
    }
}