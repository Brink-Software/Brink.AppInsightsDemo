using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    public class SomeClass
    {
        [AppInsightsRequest]
        public async Task<string> SayHello(string to)
        {
            var telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);
            string response = null;
            
            try
            {
                var greeting = $"Hello {to}";
                telemetryClient.TrackTrace($"Sending {greeting}");

                response = await SomeService.SendAsync(greeting);
            }
            catch (Exception exception)
            {
                telemetryClient.TrackException(exception);
            }

            return response;
        }
    }
}
