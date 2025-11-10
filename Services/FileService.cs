using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace lenguajevisuales2_segundoparcial.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env) => _env = env;

        public string EnsureUploadsFolder()
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
            return uploadsRoot;
        }

        public async Task<string> SaveStreamAsFileAsync(string clienteCi, string fileName, Stream fileStream)
        {
            var uploadsRoot = EnsureUploadsFolder();
            var clienteFolder = Path.Combine(uploadsRoot, clienteCi);
            if (!Directory.Exists(clienteFolder)) Directory.CreateDirectory(clienteFolder);

            var sanitized = Path.GetFileName(fileName);
            var filePath = Path.Combine(clienteFolder, sanitized);

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fs);

            // URL relativa que sirve StaticFiles
            return $"/uploads/{clienteCi}/{sanitized}";
        }

        public async Task<string> SaveUploadedFileAsync(string clienteCi, IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            return await SaveStreamAsFileAsync(clienteCi, file.FileName, ms);
        }
    }
}
