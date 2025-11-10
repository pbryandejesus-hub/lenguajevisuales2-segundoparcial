using System.ComponentModel.DataAnnotations;

namespace lenguajevisuales2_segundoparcial.Models
{
    public class Cliente
    {
        [Key]
        [Required]
        public string CI { get; set; }

        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Telefono { get; set; }

        public byte[] FotoCasa1 { get; set; }
        public byte[] FotoCasa2 { get; set; }
        public byte[] FotoCasa3 { get; set; }

        public ICollection<ArchivoCliente> Archivos { get; set; }
    }
}