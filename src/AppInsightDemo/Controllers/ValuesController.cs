using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
            var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();
            requestTelemetry.Properties.Add("key", "set from inside controller");

            // Trace some text to AppInsights. Template values are put in telemetry properties. Basically same as  _telemetryClient.TrackTrace()
            _logger.LogInformation("My logentry with some custom properties like {string}, {bool} and {guid}", "some string", true, Guid.NewGuid());

            _telemetryClient.TrackEvent("My custom event");
            _telemetryClient.TrackTrace("My custom trace");

            // Track the performance of some code rum somewhere during the request
            // using AppInsights.
            using (_telemetryClient.StartOperation<DependencyTelemetry>($"Duration.{nameof(ValuesController)}.{nameof(Get)}.GetData"))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(60));
            }

            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            // A logged exception will be routed to App Insights as well, basically like _telemetryClient.TrackException()
            _logger.LogWarning(new Exception("An exception with severity Warning"), "An error occured");

            // Unhandled exceptions will be automatically tracked by App Insights
            throw new Exception("Something went deliberately wrong.");
        }
    }
}
