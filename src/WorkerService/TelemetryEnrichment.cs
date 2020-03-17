using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace WorkerService
{
    public class TelemetryEnrichment : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = nameof(Worker);

            if (!(telemetry is ISupportProperties item)) return;

            // Demonstrate static property
            item.Properties["Environment"] = "Production";
        }
    }
}