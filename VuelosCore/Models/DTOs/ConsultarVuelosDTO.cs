using System;
using System.ComponentModel.DataAnnotations;

namespace VuelosCore.Models.DTOs
{
    public class ConsultarVuelosDTO
    {
        [Required]
        public string Uuid { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string FechaInicio { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string FechaFinal { get; set; }
        [Required]
        public string Origen { get; set; }
        [Required]
        public string Destino { get; set; }
        [Required]
        public int CantidadPasajeros { get; set; }
    }
}
