using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace NonWebIntegrationDemo
{
    public class SomeService
    {
        private readonly TelemetryClient _telemetryClient;

        public SomeService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public string Send(string greeting)
        {
            var operationName = $"{nameof(NonWebIntegrationDemo)}.{nameof(SomeService)}.{nameof(Send)}";
            using (var operation = _telemetryClient.StartOperation<DependencyTelemetry>(operationName))
            {
                try
                {
                    throw new Exception("Woopsie");
                }
                catch
                {
                    operation.Telemetry.Success = false;
                    throw;
                }
            }
        }
    }
}
