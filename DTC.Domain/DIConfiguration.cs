using DTC.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DTC.Domain
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
