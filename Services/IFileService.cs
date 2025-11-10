using Microsoft.AspNetCore.Http;

namespace lenguajevisuales2_segundoparcial.Services
{
    public interface IFileService
    {
        Task<byte[]> ReadFileToBytesAsync(IFormFile file);
        string SaveExtractedFile(string clienteCi, string fileName, Stream fileStream);
        string EnsureUploadsFolder();
    }
}