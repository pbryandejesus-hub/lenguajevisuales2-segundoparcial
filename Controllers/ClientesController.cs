using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lenguajevisuales2_segundoparcial.Data;
using lenguajevisuales2_segundoparcial.DTOs;
using lenguajevisuales2_segundoparcial.Models;

namespace lenguajevisuales2_segundoparcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ClientesController(ApplicationDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterClienteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _db.Clientes.AnyAsync(c => c.CI == dto.CI))
                return Conflict(new { message = "CI ya registrado" });

            Cliente cliente = new Cliente
            {
                CI = dto.CI,
                Nombres = dto.Nombres,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono
            };

            byte[] ToBytes(Microsoft.AspNetCore.Http.IFormFile f)
            {
                if (f == null) return null;
                using var ms = new MemoryStream();
                f.CopyTo(ms);
                return ms.ToArray();
            }

            cliente.FotoCasa1 = ToBytes(dto.FotoCasa1);
            cliente.FotoCasa2 = ToBytes(dto.FotoCasa2);
            cliente.FotoCasa3 = ToBytes(dto.FotoCasa3);

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCi), new { ci = cliente.CI }, new { cliente.CI, cliente.Nombres });
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetByCi(string ci)
        {
            var cliente = await _db.Clientes
                .Include(c => c.Archivos)
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
