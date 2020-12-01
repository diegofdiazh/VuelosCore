using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VuelosCore.Models.DTOs
{
    public class ResponseNormalaizer
    {
        public string providerName { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string price { get; set; }
        public string code { get; set; }
        public string @class { get; set; }
    }

    public class Root
    {
        public string uuid { get; set; }
        public string processType { get; set; }
        public string providerType { get; set; }
        public List<ResponseNormalaizer> providersResponse { get; set; }
    }
    public class RequestCache
    {
        public string request { get; set; }
    }

}
