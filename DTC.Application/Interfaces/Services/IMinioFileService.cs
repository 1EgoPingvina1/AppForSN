using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Http;

namespace DTC.Application.Interfaces.Services
{
    public interface IMinioFileService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName);
        Task<List<ProjectFile>> UploadProjectFilesAsync(int projectId, List<IFormFile> files, string bucket, bool isMainFile);
        Task<Stream> GetFileAsync(string fileName, string bucketName);
        Task DeleteFileAsync(string fileName, string bucketName);
    }
}
