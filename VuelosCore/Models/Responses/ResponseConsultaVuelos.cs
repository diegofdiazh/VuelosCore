using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Models.Responses
{
    public class ResponseConsultaVuelos
    {
        public List<ResponseBaseVuelos> vuelos { get; set; }
        public ResponseConsultaVuelos()
        {
            vuelos = new List<ResponseBaseVuelos>();
        }
    }
    public partial class ResponseBaseVuelos
    {
        public string Supplier { get; set; }
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime Stardate { get; set; }
        public DateTime EndDate { get; set; }
        public string Price { get; set; }
        public string FligthCode { get; set; }
      
    }
}


