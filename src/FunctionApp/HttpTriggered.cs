using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // The current request telemetry item gets a property
            var requestTelemetry = req.HttpContext.Features.Get<RequestTelemetry>();
            requestTelemetry.Properties.Add("setUsingFeature", "setUsingFeature");

            // The current telemetry item gets a property. Useful if the function is not triggered by an http request
            Activity.Current.AddTag("setUsingTag", "setUsingTag");

            // Subsequent telemetry gets this property attached
            Activity.Current.AddBaggage("setUsingActivityBaggage", "setUsingActivityBaggage"); 
            
            _telemetryClient.TrackEvent("HttpTriggered Function Executed");

            using (_telemetryClient.StartOperation<DependencyTelemetry>("aSubOperationOfHttpTriggered"))
            {
                // This dependency telemetry will have only the properties set using Activity.Current.AddBaggage(..)

                // This event telemetry will have only the properties set using Activity.Current.AddBaggage(..)
                _telemetryClient.TrackEvent("anEventInSubOperationOfHttpTriggered");
            }

            log.LogInformation("Finished Execution");

            return new OkResult();
        }
    }
}