using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lenguajevisuales2_segundoparcial.Data;
using lenguajevisuales2_segundoparcial.Models;
using lenguajevisuales2_segundoparcial.Services;
using lenguajevisuales2_segundoparcial.DTOs;

namespace lenguajevisuales2_segundoparcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileService _fileService;

        public ClientesController(ApplicationDbContext db, IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterClienteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.FotoCasa1 == null || dto.FotoCasa2 == null || dto.FotoCasa3 == null)
                return BadRequest("Se requieren FotoCasa1, FotoCasa2 y FotoCasa3.");

            if (await _db.Clientes.AnyAsync(c => c.CI == dto.CI))
                return Conflict("Cliente con esa CI ya existe.");

            var cliente = new Cliente
            {
                CI = dto.CI,
                Nombres = dto.Nombres,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                FotoCasa1 = await _fileService.ReadFileToBytesAsync(dto.FotoCasa1),
                FotoCasa2 = await _fileService.ReadFileToBytesAsync(dto.FotoCasa2),
                FotoCasa3 = await _fileService.ReadFileToBytesAsync(dto.FotoCasa3),
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCi), new { ci = cliente.CI }, new { cliente.CI, cliente.Nombres });
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetByCi(string ci)
        {
            var cliente = await _db.Clientes
                .Include(c => c.Archivos)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CI == ci);

            if (cliente == null) return NotFound();

            return Ok(new
            {
                cliente.CI,
                cliente.Nombres,
                cliente.Direccion,
                cliente.Telefono,
                Archivos = cliente.Archivos?.Select(a => new { a.IdArchivo, a.NombreArchivo, a.UrlArchivo })
            });
        }
    }
}