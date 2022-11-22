using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace monitoring.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitoringController : ControllerBase
    {

        private readonly Limits limits;
        public MonitoringController(Limits limits)
        {
            this.limits = limits;
        }

        [Route("acPower/{min}/{max}")]
        [HttpPost]
        public ActionResult acPower(float min, float max)
        {
            limits.valueLimits["acPower"] = new float[] { min, max };
            return Ok();
        }

        [Route("dcPower/{min}/{max}")]
        [HttpPost]
        public ActionResult dcPower(float min, float max)
        {
            limits.valueLimits["dcPower"] = new float[] { min, max };
            return Ok();
        }

        [Route("dailyYield/{min}/{max}")]
        [HttpPost]
        public ActionResult dailyYield(float min, float max)
        {
            limits.valueLimits["dailyYield"] = new float[] { min, max };
            return Ok();
        }

        [Route("restartLimits")]
        [HttpGet]
        public ActionResult restart()
        {
            limits.valueLimits["acPower"] = new float[] { 0.0F, 20000.20F };
            limits.valueLimits["dcPower"] = new float[] { 0.0F, 20000.20F };
            limits.valueLimits["dailyYield"] = new float[] { 0.0F, 10000.0F };
            return Ok();
        }

    }
}
