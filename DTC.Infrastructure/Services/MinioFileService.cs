using DTC.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DTC.Infrastructure.Services
{
    public class MinioFileService : IMinioFileService
    {
        private readonly IMinioClient _minio;
        private readonly ILogger<MinioFileService> _logger;
        public MinioFileService(ILogger<MinioFileService> logger, IConfiguration configuration)
        {
            _minio = new MinioClient()
                .WithEndpoint(configuration["Minio:Endpoint"])
                .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
                .WithSSL(false)
                .Build(); _logger = logger;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName)
        {
            // Создать bucket если нет
            bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));

            // Загружаем файл
            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType));

            // Возвращаем URL
            return $"{bucketName}/{fileName}";
        }

        public async Task<Stream> GetFileAsync(string fileName, string bucketName)
        {
            var ms = new MemoryStream();
            await _minio.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(ms)));

            ms.Position = 0;
            return ms;
        }

        public async Task DeleteFileAsync(string fileName, string bucketName)
        {
            await _minio.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName));
        }
    }
}
