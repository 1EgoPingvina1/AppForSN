using DTC.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
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
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioFileService> _logger;
        public MinioFileService(IMinioClient minioClient, ILogger<MinioFileService> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        private static readonly string[] AllowedExtensions = [
                    ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".zip", ".rar", ".7z", ".exe", ".pdf",
            ".doc", ".docx", ".xls", ".xlsx", ".txt"
                ];

        private static readonly long MaxFileSize = 100 * 1024 * 1024;
        
        public async Task<bool> FileExistsAsync(string bucketName, string filePath)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath);

                await _minioClient.StatObjectAsync(statObjectArgs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Stream> GetFileAsync(string bucketName, string filePath)
        {
            try
            {
                // Проверяем существование файла
                if (!await FileExistsAsync(bucketName, filePath))
                    throw new FileNotFoundException($"Файл {filePath} не найден в бакете {bucketName}");

                var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getObjectArgs);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении файла {FilePath} из бакета {BucketName}",
                    filePath, bucketName);
                throw;
            }
        }

        public async Task DeleteFileAsync(string bucketName, string filePath)
        {
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath);

                await _minioClient.RemoveObjectAsync(removeObjectArgs);

                _logger.LogInformation("Файл {FilePath} удален из бакета {BucketName}",
                    filePath, bucketName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла {FilePath} из бакета {BucketName}",
                    filePath, bucketName);
                throw;
            }
        }


        private async Task EnsureBucketExistsAsync(string bucketName)
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
                var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs);

                if (!exists)
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);

                    _logger.LogInformation("Бакет {BucketName} создан", bucketName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке/создании бакета {BucketName}", bucketName);
                throw;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string bucketName, string folderPath)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new InvalidOperationException("Файл пустой или отсутствует.");

                if (file.Length > MaxFileSize)
                    throw new InvalidOperationException("Файл превышает допустимый размер (100MB).");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                    throw new InvalidOperationException($"Недопустимое расширение файла: {extension}");

                var safeFileName = GenerateSafeFileName(file.FileName);
                var objectName = string.IsNullOrEmpty(folderPath)
                    ? safeFileName
                    : $"{folderPath.Trim('/')}/{safeFileName}";

                await EnsureBucketExistsAsync(bucketName);

                // Загружаем файл в MinIO
                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                _logger.LogInformation("Файл {FileName} успешно загружен в {BucketName}/{ObjectName}",
                    file.FileName, bucketName, objectName);

                return objectName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файла {FileName} в MinIO", file?.FileName);
                throw;
            }
        }



        private static string GenerateSafeFileName(string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            var name = Path.GetFileNameWithoutExtension(originalFileName);
            name = Regex.Replace(name, "[^a-zA-Z0-9-_]", "_");
            return $"{name}_{Guid.NewGuid():N}{ext}";
        }
    }
}
