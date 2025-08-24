using DTC.Application.AutoMapper.Mappings;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTC.Infrastructure
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration _config)
        {
            services.AddDbContext<ApplicationDataBaseContext>(options =>
                options.UseNpgsql(_config.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDataBaseContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
