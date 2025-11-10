using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace lenguajevisuales2_segundoparcial.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsRoot;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
            _uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            EnsureUploadsFolder();
        }

        public string EnsureUploadsFolder()
        {
            if (!Directory.Exists(_uploadsRoot))
                Directory.CreateDirectory(_uploadsRoot);
            return _uploadsRoot;
        }

        public async Task<byte[]> ReadFileToBytesAsync(IFormFile file)
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        public string SaveExtractedFile(string clienteCi, string fileName, Stream fileStream)
        {
            var clienteFolder = Path.Combine(_uploadsRoot, clienteCi);
            if (!Directory.Exists(clienteFolder))
                Directory.CreateDirectory(clienteFolder);

            var sanitized = Path.GetFileName(fileName);
            var filePath = Path.Combine(clienteFolder, sanitized);

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.CopyTo(fs);

            var url = $"/uploads/{clienteCi}/{sanitized}";
            return url;
        }
    }
}