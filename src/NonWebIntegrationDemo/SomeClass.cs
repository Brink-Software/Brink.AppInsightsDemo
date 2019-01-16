using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace NonWebIntegrationDemo
{
    public class SomeClass
    {
        private readonly TelemetryClient _telemetryClient;

        public SomeClass(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public string SayHello(string to)
        {
            string response = null;

            var operationName = $"{nameof(NonWebIntegrationDemo)}.{nameof(SomeClass)}.{nameof(SayHello)}";
            using (_telemetryClient.StartOperation<RequestTelemetry>(operationName))
            {
                try
                {
                    var greeting = $"Hello {to}";
                    _telemetryClient.TrackTrace($"Sending {greeting}");

                    response = new SomeService(_telemetryClient).Send(greeting);
                }
                catch (Exception exception)
                {
                    _telemetryClient.TrackException(exception);
                }

                return response;
            }
        }
    }
}
