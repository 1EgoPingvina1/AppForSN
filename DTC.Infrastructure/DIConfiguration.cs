using DTC.Application.Interfaces.Services;
using DTC.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace DTC.Infrastructure
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration _config)
        {
            services.AddMinio(configureClient =>
            {
                //configureClient
                //    .WithEndpoint(minioConfig["Endpoint"])
                //    .WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"])
                //    .WithSSL(bool.Parse(minioConfig["UseSSL"] ?? "false"));
            });

            services.AddScoped<IMinioFileService, MinioFileService>();
            return services;
        }
    }
}
