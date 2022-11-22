using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gateway_app.Models
{
    public class Currency
    {
        public int currencyId { get; set; }
        public string title { get; set; }
        public string currency { get; set; }
        public float value { get; set; }
        public string data { get; set; }
    }
}
