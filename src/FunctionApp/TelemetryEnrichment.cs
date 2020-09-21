using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace FunctionApp
{
    public class TelemetryEnrichment : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is ISupportProperties item)) return;

            // Demonstrate static property
            item.Properties["Environment"] = "Production";
        }
    }
}