using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lenguajevisuales2_segundoparcial.Data;
using lenguajevisuales2_segundoparcial.Models;
using lenguajevisuales2_segundoparcial.Services;

namespace lenguajevisuales2_segundoparcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileService _fileService;

        public ArchivosController(ApplicationDbContext db, IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        [HttpPost("upload-zip")]
        public async Task<IActionResult> UploadZip([FromQuery] string ci, IFormFile zipFile)
        {
            if (string.IsNullOrWhiteSpace(ci)) return BadRequest("Parámetro 'ci' es requerido.");
            if (zipFile == null || zipFile.Length == 0) return BadRequest("Se requiere un archivo zip.");

            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.CI == ci);
            if (cliente == null) return NotFound("Cliente no encontrado.");

            var ext = Path.GetExtension(zipFile.FileName);
            if (!string.Equals(ext, ".zip", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se acepta archivo .zip");

            using var ms = new MemoryStream();
            await zipFile.CopyToAsync(ms);
            ms.Position = 0;

            using var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false);
            var created = new List<ArchivoCliente>();

            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                using var entryStream = entry.Open();
                var url = _fileService.SaveExtractedFile(ci, entry.FullName, entryStream);

                var registro = new ArchivoCliente
                {
                    CICliente = ci,
                    NombreArchivo = entry.Name,
                    UrlArchivo = url
                };
                _db.ArchivoClientes.Add(registro);
                created.Add(registro);
            }

            await _db.SaveChangesAsync();
            return Ok(created.Select(c => new { c.IdArchivo, c.NombreArchivo, c.UrlArchivo }));
        }
    }
}