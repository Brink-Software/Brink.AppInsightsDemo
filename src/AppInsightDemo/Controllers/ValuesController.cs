using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppInsightDemo.AppInsights;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppInsightDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public ValuesController(ILogger<ValuesController> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            // Trace some text to AppInsights. Template values are put in telemetry properties
            _logger.LogInformation("My logentry with some custom properties like {string}, {bool} and {guid}", "some string", true, Guid.NewGuid());

            _telemetryClient.TrackEvent("My custom event");
            _telemetryClient.TrackTrace("My custom trace");

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
