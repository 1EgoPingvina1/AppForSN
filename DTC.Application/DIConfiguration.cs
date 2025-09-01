using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DTC.Application
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
