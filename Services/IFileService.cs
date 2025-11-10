using System.IO;

namespace lenguajevisuales2_segundoparcial.Services
{
    public interface IFileService
    {
        string EnsureUploadsFolder();
        Task<string> SaveStreamAsFileAsync(string clienteCi, string fileName, Stream fileStream);
        Task<string> SaveUploadedFileAsync(string clienteCi, IFormFile file);
    }
}
