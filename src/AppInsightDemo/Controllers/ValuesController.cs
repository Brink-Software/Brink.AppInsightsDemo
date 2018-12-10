using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppInsightDemo.AppInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppInsightDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            // Trace some text to AppInsights. Template values are put in telemetry properties
            _logger.LogInformation("My logentry with some custom properties like {string}, {bool} and {guid}", "some string", true, Guid.NewGuid());

            // Track the performance of some code rum somewhere during the request
            // using AppInsights.
            using (new DurationTracker($"Duration.{nameof(ValuesController)}.{nameof(Get)}.GetData"))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(60));
            }

            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            throw new Exception("Something went deliberately wrong.");
        }
    }
}
