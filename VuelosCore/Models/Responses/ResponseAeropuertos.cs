using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Models.Responses
{
    public class ResponseAeropuertos
    {
        public int Id { get; set; }
        public string Iata { get; set; }
        public string CiudadUbicacion { get; set; }
        public string Concatenado { get; set; }
    }
}
