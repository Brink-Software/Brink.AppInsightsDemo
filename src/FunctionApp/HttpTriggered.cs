using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace FunctionApp
{
    public class HttpTriggered
    {
        private readonly TelemetryClient _telemetryClient;

        public HttpTriggered(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [FunctionName("HttpTriggered")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            // The current request telemetry item gets a property
            var requestTelemetry = req.HttpContext.Features.Get<RequestTelemetry>();
            requestTelemetry.Properties.Add("aPropertyA", "setUsingFeature");

            // The current telemetry item gets a property. Useful if the function is not triggered by an http request
            Activity.Current.AddTag("aPropertyB", "setUsingTag");

            _telemetryClient.TrackEvent("HttpTriggered Function Executed");

            return new OkResult();
        }
    }
}