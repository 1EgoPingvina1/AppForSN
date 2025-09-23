using Microsoft.AspNetCore.Http;

namespace DTC.Application.Interfaces.Services
{
    public interface IMinioFileService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName);
        Task<Stream> GetFileAsync(string fileName, string bucketName);
        Task DeleteFileAsync(string fileName, string bucketName);
    }
}
