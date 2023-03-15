using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using OpenTelemetry;

namespace FunctionApp
{
    public class HttpTriggered
    {
        [FunctionName("HttpTriggered")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, Tracer tracer)
        {
            // The current request telemetry item gets a property
            Tracer.CurrentSpan.SetAttribute("setUsingFeature", "setUsingFeature");

            // Subsequent telemetry gets this property attached
            Baggage.SetBaggage("setUsingActivityBaggage", "setUsingActivityBaggage");

            Tracer.CurrentSpan.AddEvent("HttpTriggered Function Executed");

            using (var span = tracer.StartActiveSpan("aSubOperationOfHttpTriggered"))
            {
                // This dependency telemetry will have only the properties set using Activity.Current.AddBaggage(..)

                // This event telemetry will have only the properties set using Activity.Current.AddBaggage(..)
                span.AddEvent("anEventInSubOperationOfHttpTriggered");
            }

            // Generate an entry in the exceptions table and an entry in the trace table of severity warning with message `Some Message`
            log.LogWarning(new Exception("Demo"), "Some Message");

            log.LogInformation("Finished Execution");

            return new OkResult();
        }
    }
}