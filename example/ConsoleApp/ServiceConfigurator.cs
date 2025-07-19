using ConfigurationReader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp;

public class ServiceConfigurator
{
    private readonly IConfiguration configuration;

    public ServiceConfigurator(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigurationProvider(this.configuration);
        services.AddHostedService<ConsoleBackgroundService>();
    }
}