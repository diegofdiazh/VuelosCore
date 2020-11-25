using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Models.DTOs
{
    public class ReservaDTO
    {
        [Required]
        public string CodigoVuelo { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }

    }
}
