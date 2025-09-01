using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTC.Infrastructure
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration _config)
        {
            return services;
        }
    }
}
