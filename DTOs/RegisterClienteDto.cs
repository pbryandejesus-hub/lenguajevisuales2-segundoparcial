using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace lenguajevisuales2_segundoparcial.DTOs
{
    public class RegisterClienteDto
    {
        [Required] public string CI { get; set; }
        [Required] public string Nombres { get; set; }
        [Required] public string Direccion { get; set; }
        [Required] public string Telefono { get; set; }

        public IFormFile FotoCasa1 { get; set; }
        public IFormFile FotoCasa2 { get; set; }
        public IFormFile FotoCasa3 { get; set; }
    }
}