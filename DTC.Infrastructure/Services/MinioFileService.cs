using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Serilog;

namespace DTC.Infrastructure.Services
{
    public class MinioFileService : IMinioFileService
    {
        private readonly IMinioClient _minioStorage;
        private readonly ILogger<MinioFileService> _logger;
        public MinioFileService(ILogger<MinioFileService> logger, IMinioClient minio)
        {
            _minioStorage = minio;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName)
        {
            bool backetIsExists = await _minioStorage.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!backetIsExists)
                await _minioStorage.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));

            await _minioStorage.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType)
                );

            return $"{bucketName}/{fileName}";
        }

        public async Task<Stream> GetFileAsync(string fileName, string bucketName)
        {
            var ms = new MemoryStream();
            await _minioStorage.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(ms)));

            ms.Position = 0;
            return ms;
        }

        public async Task DeleteFileAsync(string fileName, string bucketName)
        {
            await _minioStorage.RemoveObjectAsync(new RemoveObjectArgs()
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
                    var safeFileName = $"{Guid.NewGuid()}_{file.FileName}";
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

        public async Task<string> UploadUsersAvatar(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не может быть пустым");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                throw new ArgumentException("Размер файла не должен превышать 5MB");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Допустимые форматы: JPG, JPEG, PNG, GIF");

            var fileName = $"avatar_{userId}_{Guid.NewGuid():N}{fileExtension}";
            var bucketName = "avatars";
            var objectName = $"users/{userId}/{fileName}";

            var policy = $@"{{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {{
                        ""Effect"": ""Allow"",
                        ""Principal"": {{""AWS"": [""*""]}},
                        ""Action"": [""s3:GetObject""],
                        ""Resource"": [""arn:aws:s3:::{bucketName}/*""]
                    }}
                ]
            }}";
            using var stream = file.OpenReadStream();
            var filePath = await UploadFileAsync(stream, fileName, file.ContentType, bucketName);
            await _minioStorage.SetPolicyAsync(new SetPolicyArgs().WithBucket(bucketName).WithPolicy(policy));
            return filePath;
        }
    }
}
