using Microsoft.AspNetCore.Http;

namespace DTC.Application.Interfaces.Services
{
    public interface IMinioFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string bucketName, string folderPath);
        Task<Stream> GetFileAsync(string bucketName, string filePath);
        Task DeleteFileAsync(string bucketName, string filePath);
        Task<bool> FileExistsAsync(string bucketName, string filePath);
    }
}
