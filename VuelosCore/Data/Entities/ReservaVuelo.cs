using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Data.Entities
{
    public class ReservaVuelo
    {
        public int Id { get; set; }
        public string CodigoVuelo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Token { get; set; }
        public string CodigoReserva { get; set; }
    }
}
