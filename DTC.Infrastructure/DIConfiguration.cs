using DTC.Domain.Entities;
using DTC.Domain.Interfaces;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Services;
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

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
