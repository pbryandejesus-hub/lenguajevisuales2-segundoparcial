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

        // POST api/archivos/upload-zip?ci=123
        [HttpPost("upload-zip")]
        public async Task<IActionResult> UploadZip([FromQuery] string ci, IFormFile zipFile)
        {
            if (string.IsNullOrWhiteSpace(ci)) return BadRequest("CI requerido");
            if (zipFile == null || zipFile.Length == 0) return BadRequest("Zip requerido");

            // Validar cliente
            var cliente = await _db.Clientes.FindAsync(ci);
            if (cliente == null) return NotFound("Cliente no encontrado");

            // Guardar zip temporal
            var tempPath = Path.GetTempFileName();
            using (var fs = System.IO.File.Create(tempPath))
            {
                await zipFile.CopyToAsync(fs);
            }

            var created = new List<ArchivoCliente>();

            using (var archive = ZipFile.OpenRead(tempPath))
            {
                foreach (var entry in archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
                {
                    using var entryStream = entry.Open();
                    var url = await _fileService.SaveStreamAsFileAsync(ci, entry.Name, entryStream);
                    var registro = new ArchivoCliente
                    {
                        CICliente = ci,
                        NombreArchivo = entry.Name,
                        UrlArchivo = url
                    };
                    _db.ArchivoClientes.Add(registro);
                    created.Add(registro);
                }
            }

            System.IO.File.Delete(tempPath);
            await _db.SaveChangesAsync();

            return Ok(created.Select(c => new { c.IdArchivo, c.NombreArchivo, c.UrlArchivo }));
        }

        [HttpGet("by-ci/{ci}")]
        public async Task<IActionResult> GetByCi(string ci)
        {
            var files = await _db.ArchivoClientes.Where(a => a.CICliente == ci).Select(a => new { a.IdArchivo, a.NombreArchivo, a.UrlArchivo }).ToListAsync();
            return Ok(files);
        }
    }
}
