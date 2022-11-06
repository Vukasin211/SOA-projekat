using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace publicApi.Models
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

/*
  const CurrencySchema = mongoose.Schema({
  currencyId: {
    type: Number,
    required: true,
    index: { unique: true }
  },  
  title: String,
  currency: String,
  value: Number,
  date: String
});
*/