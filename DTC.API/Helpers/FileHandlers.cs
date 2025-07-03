using System.Text.RegularExpressions;

namespace DTC.API.Helpers
{
    public static class FileHandlers
    {
        private static readonly string[] AllowedExtensions = [
         ".jpg", ".jpeg", ".png", ".gif",
        ".zip", ".rar", ".7z", ".exe"
     ];

        private static readonly long MaxFileSize = 100 * 1024 * 1024; 

        public static async Task<string> SaveFileAsync(IFormFile file, string folder, string webRootPath)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Файл пустой или отсутствует.");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("Файл превышает допустимый размер (100MB).");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Недопустимое расширение файла: {extension}");

            var safeFileName = GenerateSafeFileName(file.FileName);
            var uploadsPath = Path.Combine(webRootPath ?? "wwwroot", folder);
            EnsureDirectory(uploadsPath);

            var fullPath = Path.Combine(uploadsPath, safeFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Path.Combine(folder, safeFileName).Replace("\\", "/");
        }

        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static string GenerateSafeFileName(string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            var name = Path.GetFileNameWithoutExtension(originalFileName);
            name = Regex.Replace(name, "[^a-zA-Z0-9-_]", "_");
            return $"{name}_{Guid.NewGuid()}{ext}";
        }
    }
}

