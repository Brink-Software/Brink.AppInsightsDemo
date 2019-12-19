using System;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            TelemetryEnrichment.SetData("content", await req.ReadAsStringAsync());
            TelemetryEnrichment.SetData("method", req.Method);

            // Code below only adds context to the request telemetry and won't work for any other trigger than a HttpTrigger
            var telemetry = req.HttpContext.Features.Get<RequestTelemetry>();
            telemetry.Properties.Add("ContentLength", req.ContentLength.GetValueOrDefault().ToString());

            log.LogInformation("C# HTTP trigger function processed a request.");

            _telemetryClient.TrackEvent("Function invocated");

            await Task.Delay(TimeSpan.FromSeconds(1));

            throw new InvalidOperationException("Simulated exception");

            return new OkObjectResult("Hello World");
        }
    }
}
