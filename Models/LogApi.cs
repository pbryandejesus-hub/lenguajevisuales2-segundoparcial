using System.ComponentModel.DataAnnotations;

namespace lenguajevisuales2_segundoparcial.Models
{
    public class LogApi
    {
        [Key]
        public int IdLog { get; set; }
        public DateTime DateTime { get; set; }
        public string TipoLog { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string UrlEndpoint { get; set; }
        public string MetodoHttp { get; set; }
        public string DireccionIp { get; set; }
        public string Detalle { get; set; }
    }
}