using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Models.DTOs
{
    public class ConsultarVuelosDTO
    {
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
