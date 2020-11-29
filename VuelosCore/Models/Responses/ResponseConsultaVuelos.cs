using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VuelosCore.Models.Responses
{
    public class ResponseConsultaVuelos
    {
        public List<ResponseBase> vuelos { get; set; }
        public ResponseConsultaVuelos()
        {
            vuelos = new List<ResponseBase>();
        }
    }
    public partial class ResponseBase
    {
        public string Supplier { get; set; }
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime Stardate { get; set; }
        public DateTime EndDate { get; set; }
        public long Price { get; set; }
        public string FligthCode { get; set; }
      
    }
}


