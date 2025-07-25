﻿using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
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
            return services;
        }
    }
}
