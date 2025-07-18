using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationReader.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IAppDbContext, AppDbContext>()
                .AddScoped<IFeatureRepository, FeatureRepository>();
        }
    }
}
