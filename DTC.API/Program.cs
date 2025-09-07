using DTC.API.Helpers;
using DTC.API.middleware;
using DTC.Domain;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Services;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DTC.Application.Interfaces.Services;
using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces;
using DTC.Application.AutoMapper.Mappings;
using DTC.API.Middleware;
using Serilog;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Infrastructure.Services.RabbitMQ;
using Microsoft.Extensions.Logging.Abstractions;

namespace DTC.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDataBaseContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("TestLocalDataBase")));
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDataBaseContext>()
              .AddDefaultTokenProviders();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();
            builder.Services.AddScoped<IAuthorGroupService, AuthorGroupService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
            builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });
            builder.Services.AddDomainServices();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT авторизация. Пример: Bearer {токен}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
            }
                });
            });
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            }); 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
                };
            });
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();


            builder.Host.UseSerilog();
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<TransactionMiddleware>();
            app.UseCors("AllowSpecificOrigin");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await SeedData.InitializeAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при инициализации базы данных.");
                }
            }

            app.MapControllers();
            app.Run();

            Log.CloseAndFlush();
        }
    }
}
