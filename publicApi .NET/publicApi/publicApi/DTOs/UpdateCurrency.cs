using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace publicApi.DTOs
{
    public class UpdateCurrency
    {
        /*
        public String? Title { get; set; }
        public String? Distributor { get; set; }
        */
        public int currencyId { get; set; }
        public string title { get; set; }
        public string currency { get; set; }
        public float value { get; set; }
        public string data { get; set; }
    }
}
