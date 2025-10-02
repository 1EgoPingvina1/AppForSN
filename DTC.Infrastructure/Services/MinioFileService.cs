using DTC.Application.Interfaces;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace DTC.Infrastructure.Services
{
    public class MinioFileService : IMinioFileService
    {
        private readonly IMinioClient _minio;
        private readonly ILogger<MinioFileService> _logger;
        public MinioFileService(ILogger<MinioFileService> logger, IConfiguration configuration)
        {
            _minio = new MinioClient()
                .WithEndpoint(configuration["MinIO:Endpoint"])
                .WithCredentials(configuration["MinIO:AccessKey"], configuration["MinIO:SecretKey"])
                .WithSSL(false)
                .Build(); 
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName)
        {
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

        public async Task<List<ProjectFile>> UploadProjectFilesAsync(int projectId, List<IFormFile> files, string bucket, bool isMainFile)
        {
            var results = new List<ProjectFile>();

            if (files == null || !files.Any())
            {
                _logger.LogWarning("Пустой список файлов для bucket {BucketName}", bucket);
                return results;
            }

            // Проверяем bucket
            bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket));
            if (!found)
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));

            foreach (var file in files)
            {
                var result = new ProjectFile
                {
                    OriginalName = file.FileName,
                    Size = file.Length,
                    ContentType = file.ContentType,
                    ProjectId = projectId,
                    UploadDate = DateTime.UtcNow,
                    IsMainFile = isMainFile,
                    Backet = bucket
                };

                try
                {
                    // Генерируем безопасное имя файла
                    var safeFileName = $"{Guid.NewGuid()}_{file.FileName}";

                    // Загружаем файл в MinIO
                    using var stream = file.OpenReadStream();
                    var filePath = await UploadFileAsync(stream, safeFileName, file.ContentType, bucket);

                    result.FileName = safeFileName;

                    _logger.LogInformation("Файл {FileName} успешно загружен в bucket {BucketName} как {StoredFileName}",
                        file.FileName, bucket, safeFileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при загрузке файла {FileName} в bucket {BucketName}",
                        file.FileName, bucket);
                }

                results.Add(result);
            }

            return results;
        }
    }
}
