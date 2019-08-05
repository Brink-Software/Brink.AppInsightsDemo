using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using NonWebIntegrationDemo.AOP;

namespace NonWebIntegrationDemo
{
    public class SomeService
    {
        private readonly TelemetryClient _telemetryClient;

        public SomeService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [AppInsightsAdvice]
        public async Task<string> SendAsync(string greeting)
        {
            await Task.Delay(500);
            throw new Exception("Woopsie");
        }
    }
}
