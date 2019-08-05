using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    public class SomeClass
    {
        private readonly TelemetryClient _telemetryClient;

        public SomeClass(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [AppInsightsAdvice]
        public async Task<string> SayHello(string to)
        {
            string response = null;
            
            try
            {
                var greeting = $"Hello {to}";
                _telemetryClient.TrackTrace($"Sending {greeting}");

                response = await new SomeService(_telemetryClient).SendAsync(greeting);
            }
            catch (Exception exception)
            {
                _telemetryClient.TrackException(exception);
            }

            return response;
        }
    }
}
