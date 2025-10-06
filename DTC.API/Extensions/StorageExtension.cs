using Minio;
using System.Runtime.CompilerServices;

namespace DTC.API.Extensions
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorageService(this IServiceCollection services,IConfiguration configuration)
        {
            var config = configuration.GetSection("MinIO");
            services.AddMinio(configureClient => configureClient
                .WithEndpoint("localhost:9000")
                .WithCredentials(config["AccessKey"], config["SecretKey"])
                .WithSSL(true)
                .Build());
            return services;
        }
    }
}
