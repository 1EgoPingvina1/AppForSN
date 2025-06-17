using DTC.Domain;
using DTC.Domain.Entities;
using DTC.Domain.Interfaces;
using DTC.Infrastructure;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace DTC.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDataBaseContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDataBaseContext>()
              .AddDefaultTokenProviders();
            builder.Services.AddScoped<UserManager<User>>();
            builder.Services.AddScoped<SignInManager<User>>();
            builder.Services.AddScoped<RoleManager<Role>>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddDomainServices();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My Web API",
                    Version = "v1",
                    Description = "Description of my API",
                    Contact = new OpenApiContact
                    {
                        Name = "Developer Name",
                        Email = "dev@example.com"
                    }
                });

            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
