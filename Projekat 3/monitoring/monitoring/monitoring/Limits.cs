using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace monitoring
{
    public class Limits
    {
        public IDictionary<string, float[]> valueLimits;
        public Limits()
        {
            valueLimits = new Dictionary<string, float[]>();
            valueLimits.Add("acPower", new float[] { 0.0F, 20000.20F });
            valueLimits.Add("dcPower", new float[] { 0.0F, 20000.20F });
            valueLimits.Add("dailyYield", new float[] { 0.0F, 10000.0F });
        }
    }
}
