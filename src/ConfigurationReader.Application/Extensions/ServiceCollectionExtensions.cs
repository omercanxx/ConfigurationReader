using ConfigurationReader.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationReader.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IConfigurationService, ConfigurationService>();
        }
    }
}
