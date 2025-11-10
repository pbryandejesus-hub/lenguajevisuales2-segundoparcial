using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lenguajevisuales2_segundoparcial.Models
{
    public class ArchivoCliente
    {
        [Key]
        public int IdArchivo { get; set; }

        [Required]
        public string CICliente { get; set; }

        [Required]
        public string NombreArchivo { get; set; }

        [Required]
        public string UrlArchivo { get; set; }

        [ForeignKey(nameof(CICliente))]
        public Cliente Cliente { get; set; }
    }
}