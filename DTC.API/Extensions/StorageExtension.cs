using Minio;
using System.Runtime.CompilerServices;

namespace DTC.API.Extensions
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorageService(this IServiceCollection services)
        {
            services.AddMinio(configureClient => configureClient
                .WithEndpoint("minio:9000")
                .WithCredentials("minioadmin", "miniopassword")
                .WithSSL(false)
                .Build());
            return services;
        }
    }
}
